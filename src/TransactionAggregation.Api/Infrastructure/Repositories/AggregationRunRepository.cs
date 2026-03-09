using Microsoft.EntityFrameworkCore;
using TransactionAggregation.Api.Data;
using TransactionAggregation.Api.Domain.Entities;

namespace TransactionAggregation.Api.Infrastructure.Repositories;

public class AggregationRunRepository : IAggregationRunRepository
{
    private readonly DatabaseContext _dbContext;

    public AggregationRunRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(AggregationRun aggregationRun, CancellationToken cancellationToken = default)
    {
        await _dbContext.AggregationRuns.AddAsync(aggregationRun, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AggregationRun aggregationRun, CancellationToken cancellationToken = default)
    {
        _dbContext.AggregationRuns.Update(aggregationRun);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<AggregationRun?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.AggregationRuns.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AggregationRun>> GetRecentAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AggregationRuns.AsNoTracking()
            .OrderByDescending(x => x.StartedAtUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
