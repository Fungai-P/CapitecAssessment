using TransactionAggregation.Api.API.Requests;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Domain.Entities;
using TransactionAggregation.Api.Domain.Models;
using TransactionAggregation.Api.Models.Requests;

namespace TransactionAggregation.Api.Application.Handlers.Mappers;

public static class TransactionMapper
{
    public static AggregatedTransactionResponse Map(this AggregatedTransaction entity)
    {
        return new AggregatedTransactionResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Source = entity.Source,
            Description = entity.Description,
            Merchant = entity.Merchant,
            Amount = entity.Amount,
            Currency = entity.Currency,
            TransactionDateUtc = entity.TransactionDateUtc,
            Category = entity.Category,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }

    public static PagedResult<AggregatedTransactionResponse> Map(this PagedResult<AggregatedTransaction> pagedResult)
    {
        return new PagedResult<AggregatedTransactionResponse>
        {
            Items = pagedResult.Items.Select(x => x.Map()).ToList(),
            TotalCount = pagedResult.TotalCount,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize
        };
    }

    public static TransactionSearch Map(this TransactionSearchCommand command)
    {
        return new TransactionSearch
        {
            CustomerId = command.CustomerId,
            Category = command.Category,
            Source = command.Source,
            From = command.From,
            To = command.To,
            MinAmount = command.MinAmount,
            MaxAmount = command.MaxAmount,
            Page = command.Page,
            PageSize = command.PageSize
        };
    }

    public static TransactionSearchCommand Map(this TransactionSearchRequest request)
    {
        return new TransactionSearchCommand
        {
            CustomerId = request.CustomerId,
            Category = request.Category,
            Source = request.Source,
            From = request.From,
            To = request.To,
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
