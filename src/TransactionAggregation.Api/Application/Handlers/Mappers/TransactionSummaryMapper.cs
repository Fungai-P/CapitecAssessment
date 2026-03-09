using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Domain.Models;

namespace TransactionAggregation.Api.Application.Handlers.Mappers;

public static class TransactionSummaryMapper
{
    public static TransactionSummaryResponse Map(this TransactionSummary transactionSummary)
    {
        return new TransactionSummaryResponse
        {
            TotalDebits = transactionSummary.TotalDebits,
            NetAmount = transactionSummary.NetAmount,
            TotalCredits = transactionSummary.TotalCredits,
            TransactionCount = transactionSummary.TransactionCount
        };
    }
}
