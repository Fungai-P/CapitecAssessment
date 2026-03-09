using System.Globalization;
using TransactionAggregation.Api.Domain.External;

namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.Wallet;

public static class WalletTransactionMapper
{
    public static SourceTransaction Map(WalletRawTransaction raw)
    {
        var parsed = DateTime.Parse(
            raw.Timestamp,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

        return new SourceTransaction
        {
            SourceName = "Wallet",
            ExternalTransactionId = raw.WalletTx,
            CustomerId = raw.User,
            Amount = raw.Value,
            Currency = "ZAR",
            Description = raw.Description,
            Merchant = raw.Description,
            TransactionDateUtc = DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
        };
    }
}
