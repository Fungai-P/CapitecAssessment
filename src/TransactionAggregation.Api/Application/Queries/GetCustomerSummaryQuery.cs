using MediatR;
using TransactionAggregation.Api.API.Responses;

namespace TransactionAggregation.Api.Application.Queries;

public class GetCustomerSummaryQuery : IRequest<CustomerSummaryResponse>
{
    public string CustomerId { get; }
    public GetCustomerSummaryQuery(string customerId)
    {
        CustomerId = customerId;
    }
}
