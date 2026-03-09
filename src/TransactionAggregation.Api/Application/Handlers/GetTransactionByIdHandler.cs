using MediatR;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Handlers.Mappers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Infrastructure.Repositories;

namespace TransactionAggregation.Api.Application.Handlers;

public interface IGetTransactionByIdHandler
{
    Task<IReadOnlyCollection<AggregatedTransactionResponse>> HandleAsync(string customerId, CancellationToken cancellationToken);
}

public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, IReadOnlyCollection<AggregatedTransactionResponse>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionByIdHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IReadOnlyCollection<AggregatedTransactionResponse>> Handle(GetTransactionByIdQuery query, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetByCustomerAsync(query.CustomerId, cancellationToken);

        return transactions.Select(x => x.Map()).ToList();
    }
}
