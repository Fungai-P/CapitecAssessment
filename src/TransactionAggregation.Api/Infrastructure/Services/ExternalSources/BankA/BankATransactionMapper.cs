using TransactionAggregation.Api.Domain.External;

namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.BankA;

public static class BankATransactionMapper
{
    public static SourceTransaction Map(BankARawTransaction raw)
    {
        var amount = raw.DebitAmount != 0
            ? -raw.DebitAmount
            : raw.CreditAmount;

        return new SourceTransaction
        {
            SourceName = "BankA",
            ExternalTransactionId = raw.Id,
            CustomerId = raw.AccountHolder,
            Amount = amount,
            Currency = "ZAR",
            Description = raw.Notes,
            Merchant = raw.MerchantName,
            TransactionDateUtc = raw.DateUtc
        };
    }
}

