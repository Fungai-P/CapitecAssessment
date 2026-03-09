namespace TransactionAggregation.Api.Domain.Models;

public class CategorySummary
{
    public string Category { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}
