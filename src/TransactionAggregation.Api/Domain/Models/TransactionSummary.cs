namespace TransactionAggregation.Api.Domain.Models;

public class TransactionSummary
{
    public int TransactionCount { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal NetAmount { get; set; }
}
