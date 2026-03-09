using MediatR;
using TransactionAggregation.Api.API.Responses;

namespace TransactionAggregation.Api.Models.Requests;

public class TransactionSearchCommand : IRequest<PagedResult<AggregatedTransactionResponse>>
{
    public string CustomerId { get; set; }
    public string? Category { get; set; }
    public string? Source { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
