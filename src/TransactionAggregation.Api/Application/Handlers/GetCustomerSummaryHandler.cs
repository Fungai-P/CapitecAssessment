using MediatR;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Handlers.Mappers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Infrastructure.Repositories;

namespace TransactionAggregation.Api.Application.Handlers;

public class GetCustomerSummaryHandler : IRequestHandler<GetCustomerSummaryQuery, CustomerSummaryResponse>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetCustomerSummaryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<CustomerSummaryResponse> Handle(GetCustomerSummaryQuery query, CancellationToken cancellationToken)
    {
        var result = await _transactionRepository.GetCustomerSummaryAsync(query.CustomerId, cancellationToken);
        return result.Map();
    }
}
