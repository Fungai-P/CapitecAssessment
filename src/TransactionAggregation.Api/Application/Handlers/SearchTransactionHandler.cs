using MediatR;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Handlers.Mappers;
using TransactionAggregation.Api.Infrastructure.Repositories;
using TransactionAggregation.Api.Models.Requests;

namespace TransactionAggregation.Api.Application.Handlers;

public class SearchTransactionHandler : IRequestHandler<TransactionSearchCommand, PagedResult<AggregatedTransactionResponse>>
{
    private readonly ITransactionRepository _transactionRepository;

    public SearchTransactionHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<PagedResult<AggregatedTransactionResponse>> Handle(TransactionSearchCommand command, CancellationToken cancellationToken)
    {
        var result = await _transactionRepository.SearchAsync(command.Map(), cancellationToken);

        return result.Map();
    }
}
