using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Domain.Models;

namespace TransactionAggregation.Api.Application.Handlers.Mappers;

public static class CustomerSummaryMapper
{
    public static CustomerSummaryResponse Map(this CustomerSummary customerSummary)
    {
        return new CustomerSummaryResponse
        {
            CustomerId = customerSummary.CustomerId,
            TransactionCount = customerSummary.TransactionCount,
            NetAmount = customerSummary.NetAmount,
            TotalCredits = customerSummary.TotalCredits,
            TotalDebits = customerSummary.TotalDebits
        };
    }
}
