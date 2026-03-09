using MediatR;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Handlers.Mappers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Infrastructure.Repositories;

namespace TransactionAggregation.Api.Application.Handlers;

public class GetOverallSummaryHandler : IRequestHandler<GetOverallSummaryQuery, TransactionSummaryResponse>
{
    private readonly ITransactionRepository _transactionRepository;
    
    public GetOverallSummaryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<TransactionSummaryResponse> Handle(GetOverallSummaryQuery query, CancellationToken cancellationToken)
    {
        var result = await _transactionRepository.GetOverallSummaryAsync(cancellationToken);

        return result.Map();
    }
}
