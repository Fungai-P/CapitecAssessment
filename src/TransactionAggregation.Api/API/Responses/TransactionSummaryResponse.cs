namespace TransactionAggregation.Api.API.Responses;

public class TransactionSummaryResponse
{
    public int TransactionCount { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal NetAmount { get; set; }
}
