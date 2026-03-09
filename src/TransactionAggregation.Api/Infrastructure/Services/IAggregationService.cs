namespace TransactionAggregation.Api.Infrastructure.Services;

public interface IAggregationService
{
    Task AggregateAsync(string triggeredBy, CancellationToken cancellationToken = default);
}
