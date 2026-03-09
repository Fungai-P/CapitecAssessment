namespace TransactionAggregation.Api.API.Responses;

public class AggregatedTransactionResponse
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public string Source { get; set; }
    public string Description { get; set; }
    public string? Merchant { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public DateTime TransactionDateUtc { get; set; }
    public string Category { get; set; } = "Uncategorized";
    public DateTime CreatedAtUtc { get; set; }
}
