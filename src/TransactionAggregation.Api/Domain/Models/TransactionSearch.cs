namespace TransactionAggregation.Api.Domain.Models;

public class TransactionSearch
{
    public string? CustomerId { get; set; }
    public string? Category { get; set; }
    public string? Source { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
