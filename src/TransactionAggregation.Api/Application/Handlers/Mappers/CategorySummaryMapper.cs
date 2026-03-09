using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Domain.Models;

namespace TransactionAggregation.Api.Application.Handlers.Mappers;

public static class CategorySummaryMapper
{
    public static CategorySummaryResponse Map(this CategorySummary categorySummary)
    {
        return new CategorySummaryResponse
        {
            Category = categorySummary.Category,
            TransactionCount = categorySummary.TransactionCount,
            TotalAmount = categorySummary.TotalAmount            
        };
    }
}
