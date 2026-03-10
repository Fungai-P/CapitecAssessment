using TransactionAggregation.Api.Domain.Enums;

namespace TransactionAggregation.Api.Domain.Entities;

public class AggregationRun
{
    public Guid Id { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public AggregationRunStatus Status { get; set; }
    public int RecordsFetched { get; set; }
    public int RecordsInserted { get; set; }
    public int RecordsUpdated { get; set; }
    public int RecordsSkipped { get; set; }
    public string TriggeredBy { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
