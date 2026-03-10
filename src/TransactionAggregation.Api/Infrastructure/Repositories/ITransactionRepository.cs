using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Domain.Entities;
using TransactionAggregation.Api.Domain.Models;

namespace TransactionAggregation.Api.Infrastructure.Repositories;

public interface ITransactionRepository
{
    Task<PagedResult<AggregatedTransaction>> SearchAsync(TransactionSearch transactionSearch, CancellationToken cancellationToken = default);
    Task<AggregatedTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AggregatedTransaction>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken = default);
    Task<CustomerSummary> GetCustomerSummaryAsync(string customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CategorySummary>> GetCategorySummaryAsync(CancellationToken cancellationToken = default);
    Task<TransactionSummary> GetOverallSummaryAsync(CancellationToken cancellationToken = default);
    Task<int> InsertManyIgnoreDuplicatesAsync(
        IReadOnlyCollection<AggregatedTransaction> transactions,
        CancellationToken cancellationToken = default);
}
