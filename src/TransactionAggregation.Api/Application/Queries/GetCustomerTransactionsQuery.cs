using MediatR;
using TransactionAggregation.Api.API.Responses;

namespace TransactionAggregation.Api.Application.Queries;

public class GetCustomerTransactionsQuery : IRequest<IReadOnlyCollection<AggregatedTransactionResponse>>
{
    public string CustomerId { get; }
    public GetCustomerTransactionsQuery(string customerId)
    {
        CustomerId = customerId;
    }
}
