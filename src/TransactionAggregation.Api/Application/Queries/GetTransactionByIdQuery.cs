using MediatR;
using TransactionAggregation.Api.API.Responses;

namespace TransactionAggregation.Api.Application.Queries;

public class GetTransactionByIdQuery : IRequest<IReadOnlyCollection<AggregatedTransactionResponse>>
{
    public string CustomerId { get; }
    public GetTransactionByIdQuery(string customerId)
    {
        CustomerId = customerId;
    }
}
