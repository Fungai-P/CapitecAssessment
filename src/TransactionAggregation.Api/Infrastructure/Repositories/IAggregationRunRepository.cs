using TransactionAggregation.Api.Domain.Entities;

namespace TransactionAggregation.Api.Infrastructure.Repositories;

public interface IAggregationRunRepository
{
    Task AddAsync(AggregationRun aggregationRun, CancellationToken cancellationToken = default);
    Task UpdateAsync(AggregationRun aggregationRun, CancellationToken cancellationToken = default);
    Task<AggregationRun?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AggregationRun>> GetRecentAsync(int limit, CancellationToken cancellationToken = default);
}
