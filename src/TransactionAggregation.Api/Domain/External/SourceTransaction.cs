namespace TransactionAggregation.Api.Domain.External;

public class SourceTransaction
{
    public string CustomerId { get; set; }
    public string ExternalTransactionId { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Merchant { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public DateTime TransactionDateUtc { get; set; }
}
