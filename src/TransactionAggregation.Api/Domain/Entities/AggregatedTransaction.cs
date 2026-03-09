namespace TransactionAggregation.Api.Domain.Entities;

public class AggregatedTransaction
{
    public Guid Id { get; set; }
    public string IdempotencyKey { get; set; } = "";
    public string CustomerId { get; set; } = "";
    public string ExternalTransactionId { get; set; } = "";
    public string Source { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "";
    public string Category { get; set; } = "";
    public string Merchant { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime TransactionDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
