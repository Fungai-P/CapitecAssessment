using Microsoft.EntityFrameworkCore;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Data;
using TransactionAggregation.Api.Domain.Entities;
using TransactionAggregation.Api.Domain.Models;

namespace TransactionAggregation.Api.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly DatabaseContext _dbContext;

    public TransactionRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UpsertResult> UpsertManyAsync(IReadOnlyCollection<AggregatedTransaction> transactions, CancellationToken cancellationToken = default)
    {
        var result = new UpsertResult();

        foreach (var transaction in transactions)
        {
            var existing = await _dbContext.AggregatedTransactions
                .FirstOrDefaultAsync(
                    x => x.Source == transaction.Source && x.ExternalTransactionId == transaction.ExternalTransactionId,
                    cancellationToken);

            if (existing is null)
            {
                await _dbContext.AggregatedTransactions.AddAsync(transaction, cancellationToken);
                result.Inserted++;
            }
            else
            {
                existing.CustomerId = transaction.CustomerId;
                existing.Description = transaction.Description;
                existing.Merchant = transaction.Merchant;
                existing.Amount = transaction.Amount;
                existing.Currency = transaction.Currency;
                existing.TransactionDateUtc = transaction.TransactionDateUtc;
                existing.Category = transaction.Category;
                result.Updated++;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return result;
    }

    public async Task<PagedResult<AggregatedTransaction>> SearchAsync(TransactionSearch transactionSearch, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AggregatedTransactions.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(transactionSearch.CustomerId))
            query = query.Where(x => x.CustomerId == transactionSearch.CustomerId);

        if (!string.IsNullOrWhiteSpace(transactionSearch.Category))
            query = query.Where(x => x.Category == transactionSearch.Category);

        if (!string.IsNullOrWhiteSpace(transactionSearch.Source))
            query = query.Where(x => x.Source == transactionSearch.Source);

        if (transactionSearch.From.HasValue)
            query = query.Where(x => x.TransactionDateUtc >= transactionSearch.From.Value);

        if (transactionSearch.To.HasValue)
            query = query.Where(x => x.TransactionDateUtc <= transactionSearch.To.Value);

        if (transactionSearch.MinAmount.HasValue)
            query = query.Where(x => x.Amount >= transactionSearch.MinAmount.Value);

        if (transactionSearch.MaxAmount.HasValue)
            query = query.Where(x => x.Amount <= transactionSearch.MaxAmount.Value);

        var page = transactionSearch.Page <= 0 ? 1 : transactionSearch.Page;
        var pageSize = transactionSearch.PageSize <= 0 ? 20 : Math.Min(transactionSearch.PageSize, 200);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.TransactionDateUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AggregatedTransaction>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<AggregatedTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.AggregatedTransactions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AggregatedTransaction>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AggregatedTransactions.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.TransactionDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerSummary> GetCustomerSummaryAsync(string customerId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AggregatedTransactions.AsNoTracking().Where(x => x.CustomerId == customerId);

        return new CustomerSummary
        {
            CustomerId = customerId,
            TransactionCount = await query.CountAsync(cancellationToken),
            TotalCredits = await query.Where(x => x.Amount > 0).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,
            TotalDebits = await query.Where(x => x.Amount < 0).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,
            NetAmount = await query.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0
        };
    }

    public async Task<IReadOnlyCollection<CategorySummary>> GetCategorySummaryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.AggregatedTransactions.AsNoTracking()
            .GroupBy(x => x.Category)
            .Select(group => new CategorySummary
            {
                Category = group.Key,
                TransactionCount = group.Count(),
                TotalAmount = group.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TransactionCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<TransactionSummary> GetOverallSummaryAsync(CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AggregatedTransactions.AsNoTracking();

        return new TransactionSummary
        {
            TransactionCount = await query.CountAsync(cancellationToken),
            TotalCredits = await query.Where(x => x.Amount > 0).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,
            TotalDebits = await query.Where(x => x.Amount < 0).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,
            NetAmount = await query.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0
        };
    }

    public async Task<int> InsertManyIgnoreDuplicatesAsync(
        List<AggregatedTransaction> transactions,
        CancellationToken cancellationToken = default)
    {
        if (transactions.Count == 0)
            return 0;

        const int batchSize = 200;
        var inserted = 0;

        for (var i = 0; i < transactions.Count; i += batchSize)
        {
            var batch = transactions.Skip(i).Take(batchSize).ToList();
            inserted += await InsertBatchIgnoreDuplicatesAsync(batch, cancellationToken);
        }

        return inserted;
    }

    private async Task<int> InsertBatchIgnoreDuplicatesAsync(
        List<AggregatedTransaction> batch,
        CancellationToken cancellationToken)
    {
        var sql =
            """
            INSERT INTO "Transactions"
            ("Id", "IdempotencyKey", "CustomerId", "ExternalTransactionId", "Source", "Amount", "Currency", "Category", "Merchant", "Description", "TransactionDateUtc", "CreatedAtUtc")
            VALUES
            """ + string.Join(",",
                batch.Select((x, i) =>
                    $"(@p{i}_id, @p{i}_key, @p{i}_customerId, @p{i}_externalId, @p{i}_source, @p{i}_amount, @p{i}_currency, @p{i}_category, @p{i}_merchant, @p{i}_description, @p{i}_date, @p{i}_created)")) +
            """
            ON CONFLICT ("IdempotencyKey") DO NOTHING;
            """;

        var parameters = new List<object>();

        for (var i = 0; i < batch.Count; i++)
        {
            var item = batch[i];

            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_id", item.Id));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_key", item.IdempotencyKey));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_customerId", item.CustomerId));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_externalId", item.ExternalTransactionId));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_source", item.Source));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_amount", item.Amount));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_currency", item.Currency));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_category", item.Category));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_merchant", item.Merchant));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_description", item.Description));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_date", item.TransactionDateUtc));
            parameters.Add(new Npgsql.NpgsqlParameter($"p{i}_created", item.CreatedAtUtc));
        }

        return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters.ToArray(), cancellationToken);
    }
}
