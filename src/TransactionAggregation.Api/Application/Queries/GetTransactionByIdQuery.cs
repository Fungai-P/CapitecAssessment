using MediatR;
using TransactionAggregation.Api.API.Responses;

namespace TransactionAggregation.Api.Application.Queries;

public class GetTransactionByIdQuery : IRequest<AggregatedTransactionResponse>
{
    public Guid Id { get; }
    public GetTransactionByIdQuery(Guid id)
    {
        Id = id;
    }
}
