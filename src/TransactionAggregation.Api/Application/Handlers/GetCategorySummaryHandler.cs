using MediatR;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Handlers.Mappers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Infrastructure.Repositories;

namespace TransactionAggregation.Api.Application.Handlers;

public class GetCategorySummaryHandler : IRequestHandler<GetCategorySummaryQuery, IReadOnlyCollection<CategorySummaryResponse>>
{
        private readonly ITransactionRepository _transactionRepository;
    
        public GetCategorySummaryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
    
        public async Task<IReadOnlyCollection<CategorySummaryResponse>> Handle(GetCategorySummaryQuery query, CancellationToken cancellationToken)
        {
            var result = await _transactionRepository.GetCategorySummaryAsync(cancellationToken);

        return result.Select(x => x.Map()).ToList();
    }
}
