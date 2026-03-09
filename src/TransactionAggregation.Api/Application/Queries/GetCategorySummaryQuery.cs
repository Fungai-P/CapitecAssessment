using MediatR;
using TransactionAggregation.Api.API.Responses;

namespace TransactionAggregation.Api.Application.Queries;

public class GetCategorySummaryQuery : IRequest<IReadOnlyCollection<CategorySummaryResponse>>
{

}
