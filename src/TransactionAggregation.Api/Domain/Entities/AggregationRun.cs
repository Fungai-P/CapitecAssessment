namespace TransactionAggregation.Api.Domain.Entities;

public class AggregationRun
{
    public Guid Id { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public int RecordsFetched { get; set; }
    public int RecordsInserted { get; set; }
    public int RecordsUpdated { get; set; }
    public int RecordsSkipped { get; set; }
    public string TriggeredBy { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
