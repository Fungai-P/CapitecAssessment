using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TransactionAggregation.Api.Infrastructure.Scheduled.Jobs;

namespace TransactionAggregation.Api.Controllers;

[ApiController]
[Route("api/aggregation")]
public class AggregationController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public AggregationController(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    [HttpPost("run")]
    public IActionResult RunManualAggregation()
    {
        var jobId = _backgroundJobClient.Enqueue<AggregateTransactionsJob>(job => job.RunAsync("Manual"));
        return Accepted(new { JobId = jobId, Message = "Aggregation job queued successfully." });
    }
}
