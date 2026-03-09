using TransactionAggregation.Api.Infrastructure.Services;

namespace TransactionAggregation.Api.Infrastructure.Scheduled.Jobs;

public class AggregateTransactionsJob
{
    private readonly IAggregationService _aggregationService;

    public AggregateTransactionsJob(IAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    public async Task RunAsync(string triggeredBy = "System")
    {
        await _aggregationService.AggregateAsync(triggeredBy);
    }
}
