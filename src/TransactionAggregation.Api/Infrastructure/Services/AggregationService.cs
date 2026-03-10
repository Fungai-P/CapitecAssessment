using TransactionAggregation.Api.Domain.Entities;
using TransactionAggregation.Api.Domain.Enums;
using TransactionAggregation.Api.Domain.External;
using TransactionAggregation.Api.Infrastructure.Repositories;

namespace TransactionAggregation.Api.Infrastructure.Services;

public class AggregationService : IAggregationService
{
    private readonly IEnumerable<ITransactionSourceClient> _transactionSourceClients;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAggregationRunRepository _aggregationRunRepository;
    private readonly ITransactionCategorizationService _transactionCategorizationService;
    private readonly ITransactionIdempotencyService _transactionIdempotencyService;

    public AggregationService(
        IEnumerable<ITransactionSourceClient> transactionSourceClients,
        ITransactionRepository transactionRepository,
        IAggregationRunRepository aggregationRunRepository,
        ITransactionCategorizationService transactionCategorizationService,
        ITransactionIdempotencyService transactionIdempotencyService)
    {
        _transactionSourceClients = transactionSourceClients;
        _transactionRepository = transactionRepository;
        _aggregationRunRepository = aggregationRunRepository;
        _transactionCategorizationService = transactionCategorizationService;
        _transactionIdempotencyService = transactionIdempotencyService;
    }

    public async Task AggregateAsync(string triggeredBy, CancellationToken cancellationToken = default)
    {
        var run = new AggregationRun
        {
            Id = Guid.NewGuid(),
            StartedAtUtc = DateTime.UtcNow,
            Status = AggregationRunStatus.Running,
            TriggeredBy = triggeredBy
        };

        await _aggregationRunRepository.AddAsync(run, cancellationToken);

        try
        {
            var fetchedTransactions = new List<SourceTransaction>();

            foreach (var sourceClient in _transactionSourceClients)
            {
                var sourceTransactions = await sourceClient.GetTransactionsAsync(cancellationToken);
                fetchedTransactions.AddRange(sourceTransactions);
            }

            var nowUtc = DateTime.UtcNow;

            // Collapse duplicates within the same aggregation run before hitting the DB.
            var distinctSourceTransactions = fetchedTransactions
                .Select(sourceTransaction => new
                {
                    SourceTransaction = NormalizeSourceTransaction(sourceTransaction),
                    IdempotencyKey = _transactionIdempotencyService.Build(sourceTransaction)
                })
                .GroupBy(x => x.IdempotencyKey)
                .Select(group => group.First().SourceTransaction)
                .ToList();

            var aggregatedTransactions = distinctSourceTransactions
                .Select(sourceTransaction => new AggregatedTransaction
                {
                    Id = Guid.NewGuid(),
                    IdempotencyKey = _transactionIdempotencyService.Build(NormalizeSourceTransaction(sourceTransaction)), // Enforce idempotency
                    CustomerId = sourceTransaction.CustomerId,
                    ExternalTransactionId = sourceTransaction.ExternalTransactionId,
                    Source = sourceTransaction.SourceName,
                    Description = sourceTransaction.Description,
                    Merchant = sourceTransaction.Merchant,
                    Amount = sourceTransaction.Amount,
                    Currency = sourceTransaction.Currency,
                    TransactionDateUtc = EnsureUtc(sourceTransaction.TransactionDateUtc),
                    Category = _transactionCategorizationService.Categorize(
                        sourceTransaction.Description,
                        sourceTransaction.Merchant,
                        sourceTransaction.Amount),
                    CreatedAtUtc = nowUtc
                })
                .ToList();

            var insertResult = await _transactionRepository.InsertManyIgnoreDuplicatesAsync(aggregatedTransactions, cancellationToken);

            run.CompletedAtUtc = DateTime.UtcNow;
            run.Status = AggregationRunStatus.Completed;
            run.RecordsFetched = fetchedTransactions.Count;
            run.RecordsInserted = insertResult;
            run.RecordsUpdated = 0;
            run.RecordsSkipped = fetchedTransactions.Count - insertResult;
            run.ErrorMessage = null;

            await _aggregationRunRepository.UpdateAsync(run, cancellationToken);
        }
        catch (Exception ex)
        {
            run.CompletedAtUtc = DateTime.UtcNow;
            run.Status = AggregationRunStatus.Failed;
            run.ErrorMessage = ex.Message;

            await _aggregationRunRepository.UpdateAsync(run, cancellationToken);
            throw;
        }
    }

    private SourceTransaction NormalizeSourceTransaction(SourceTransaction sourceTransaction)
    {
        return new SourceTransaction
        {
            SourceName = sourceTransaction.SourceName?.Trim() ?? string.Empty,
            ExternalTransactionId = sourceTransaction.ExternalTransactionId?.Trim() ?? string.Empty,
            CustomerId = sourceTransaction.CustomerId?.Trim() ?? string.Empty,
            Amount = Math.Round(sourceTransaction.Amount, 2),
            Currency = sourceTransaction.Currency?.Trim().ToUpperInvariant() ?? string.Empty,
            Description = sourceTransaction.Description?.Trim() ?? string.Empty,
            Merchant = sourceTransaction.Merchant?.Trim() ?? string.Empty,
            TransactionDateUtc = EnsureUtc(sourceTransaction.TransactionDateUtc)
        };
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}