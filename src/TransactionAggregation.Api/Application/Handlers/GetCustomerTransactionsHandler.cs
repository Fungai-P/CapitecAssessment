using MediatR;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Handlers.Mappers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Infrastructure.Repositories;

namespace TransactionAggregation.Api.Application.Handlers;

public class GetCustomerTransactionsHandler : IRequestHandler<GetCustomerTransactionsQuery, IReadOnlyCollection<AggregatedTransactionResponse>>   
{
        private readonly ITransactionRepository _transactionRepository;
    
        public GetCustomerTransactionsHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
    
        public async Task<IReadOnlyCollection<AggregatedTransactionResponse>> Handle(GetCustomerTransactionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _transactionRepository.GetByCustomerAsync(query.CustomerId, cancellationToken);

            return result.Select(x => x.Map()).ToList();
    }
}
