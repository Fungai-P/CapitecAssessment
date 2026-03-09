using Hangfire;
using TransactionAggregation.Api.Infrastructure.Scheduled.Jobs;

namespace TransactionAggregation.Api.Infrastructure.Scheduled;

public static class Scheduler
{
    public static void Run()
    {
        RecurringJob.AddOrUpdate<AggregateTransactionsJob>(
            recurringJobId: "aggregate-transactions",
            methodCall: job => job.RunAsync("System"),
            cronExpression: "*/15 * * * *");
    }
}
