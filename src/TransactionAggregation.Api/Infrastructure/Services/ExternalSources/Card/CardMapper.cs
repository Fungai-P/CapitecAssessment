using TransactionAggregation.Api.Domain.External;

namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.Card;

public static class CardMapper
{
    public static SourceTransaction Map(CardRawTransaction raw)
    {
        return new SourceTransaction
        {
            SourceName = "VISA",
            ExternalTransactionId = raw.CardId,
            CustomerId = raw.CustomerRef,
            Amount = raw.Value,
            Currency = "ZAR",
            Merchant = raw.Vendor,
            Description = raw.Vendor,
            TransactionDateUtc = raw.ProcessedAt.ToUniversalTime()
        };
    }
}
