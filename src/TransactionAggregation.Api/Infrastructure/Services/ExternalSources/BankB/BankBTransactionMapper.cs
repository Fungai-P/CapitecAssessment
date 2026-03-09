using TransactionAggregation.Api.Domain.External;

namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.BankB;

public static class BankBTransactionMapper
{
    public static SourceTransaction Map(BankBRawTransaction raw)
    {
        return new SourceTransaction
        {
            SourceName = "BankB",
            ExternalTransactionId = raw.TxId,
            CustomerId = raw.Customer,
            Amount = raw.Amount,
            Currency = "ZAR",
            Merchant = raw.Merchant,
            Description = raw.Merchant,
            TransactionDateUtc =
                DateTimeOffset.FromUnixTimeSeconds(raw.UnixTimestamp).UtcDateTime
        };
    }
}
