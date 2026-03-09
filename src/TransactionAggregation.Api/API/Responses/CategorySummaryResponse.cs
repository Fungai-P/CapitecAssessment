namespace TransactionAggregation.Api.API.Responses;

public class CategorySummaryResponse
{
    public string Category { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}
