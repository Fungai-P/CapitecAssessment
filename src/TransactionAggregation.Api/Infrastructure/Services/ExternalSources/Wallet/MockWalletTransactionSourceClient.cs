using TransactionAggregation.Api.Domain.External;
using TransactionAggregation.Api.Infrastructure.Services;

namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.Wallet;
public class MockWalletTransactionSourceClient : ITransactionSourceClient
{
    public Task<List<SourceTransaction>> GetTransactionsAsync(CancellationToken cancellationToken = default)
    {
        var raw = new List<WalletRawTransaction>
        {
            new()
            {
                WalletTx = "W1",
                User = "customer-1",
                Value = -20,
                Description = "Netflix",
                Timestamp = DateTime.UtcNow.AddDays(-1).ToString("O")
            },
            new()
            {
                WalletTx = "W2",
                User = "customer-1",
                Value = -15,
                Description = "Spotify",
                Timestamp = DateTime.UtcNow.AddDays(-2).ToString("O")
            },
            new()
            {
                WalletTx = "W3",
                User = "customer-2",
                Value = -30,
                Description = "Uber",
                Timestamp = DateTime.UtcNow.AddDays(-1).ToString("O")
            }
        };

        return Task.FromResult(raw.Select(WalletTransactionMapper.Map).ToList());
    }
}
