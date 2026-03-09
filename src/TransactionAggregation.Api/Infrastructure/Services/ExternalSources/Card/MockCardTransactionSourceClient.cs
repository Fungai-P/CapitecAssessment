using TransactionAggregation.Api.Domain.External;
using TransactionAggregation.Api.Infrastructure.Services;

namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.Card;

public class MockCardTransactionSourceClient : ITransactionSourceClient
{
    public Task<List<SourceTransaction>> GetTransactionsAsync(CancellationToken cancellationToken = default)
    {
        var raw = new List<CardRawTransaction>
        {
            // SAME Uber ride as BankB B1
            new()
            {
                CardId = "CN1",
                CustomerRef = "customer-1",
                Value = -60,
                Vendor = "Uber",
                ProcessedAt = DateTime.UtcNow.AddDays(-1)
            },

            // SAME Netflix payment as Wallet W1
            new()
            {
                CardId = "CN2",
                CustomerRef = "customer-1",
                Value = -20,
                Vendor = "Netflix",
                ProcessedAt = DateTime.UtcNow.AddDays(-1)
            },

            // Unique transaction
            new()
            {
                CardId = "CN3",
                CustomerRef = "customer-2",
                Value = -120,
                Vendor = "Mr D Food",
                ProcessedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        return Task.FromResult(raw.Select(CardMapper.Map).ToList());
    }
}
