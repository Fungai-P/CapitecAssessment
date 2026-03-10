using MediatR;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Handlers.Mappers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Infrastructure.Repositories;

namespace TransactionAggregation.Api.Application.Handlers;

public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, AggregatedTransactionResponse>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionByIdHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<AggregatedTransactionResponse> Handle(GetTransactionByIdQuery query, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(query.Id, cancellationToken);

        return transaction.Map();
    }
}
