using TransactionAggregation.Api.Domain.External;
using TransactionAggregation.Api.Infrastructure.Services;

namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.BankB;

public class MockBankBTransactionSourceClient : ITransactionSourceClient
{
    public Task<List<SourceTransaction>> GetTransactionsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        var raw = new List<BankBRawTransaction>
        {
            new()
            {
                TxId = "B1",
                Customer = "customer-1",
                Amount = -60,
                Merchant = "Uber",
                UnixTimestamp = now.AddDays(-1).ToUnixTimeSeconds()
            },
            new()
            {
                TxId = "B2",
                Customer = "customer-1",
                Amount = -140,
                Merchant = "Checkers",
                UnixTimestamp = now.AddDays(-2).ToUnixTimeSeconds()
            },
            new()
            {
                TxId = "B3",
                Customer = "customer-2",
                Amount = -500,
                Merchant = "Shell",
                UnixTimestamp = now.AddDays(-3).ToUnixTimeSeconds()
            },
            new()
            {
                TxId = "B4",
                Customer = "customer-2",
                Amount = -200,
                Merchant = "Woolworths",
                UnixTimestamp = now.AddDays(-4).ToUnixTimeSeconds()
            },
            new()
            {
                TxId = "B5",
                Customer = "cust-3",
                Amount = -120,
                Merchant = "Netflix",
                UnixTimestamp = now.AddDays(-5).ToUnixTimeSeconds()
            }
        };

        return Task.FromResult(raw.Select(BankBTransactionMapper.Map).ToList());
    }
}
