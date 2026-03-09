using TransactionAggregation.Api.Domain.External;
using TransactionAggregation.Api.Infrastructure.Services;

namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.BankA;

public class MockBankATransactionSourceClient : ITransactionSourceClient
{
    public Task<List<SourceTransaction>> GetTransactionsAsync(CancellationToken cancellationToken = default)
    {
        var raw = new List<BankARawTransaction>
        {
            new()
            {
                Id = "A1",
                AccountHolder = "customer-1",
                DebitAmount = 120,
                MerchantName = "Checkers",
                Notes = "Groceries",
                DateUtc = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = "A2",
                AccountHolder = "customer-1",
                DebitAmount = 80,
                MerchantName = "Pick n Pay",
                Notes = "Groceries",
                DateUtc = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                Id = "A3",
                AccountHolder = "customer-2",
                DebitAmount = 600,
                MerchantName = "Engen",
                Notes = "Fuel",
                DateUtc = DateTime.UtcNow.AddDays(-3)
            },
            new()
            {
                Id = "A4",
                AccountHolder = "customer-1",
                CreditAmount = 12000,
                MerchantName = "Employer",
                Notes = "Salary",
                DateUtc = DateTime.UtcNow.AddDays(-8)
            }
        };

        return Task.FromResult(raw.Select(BankATransactionMapper.Map).ToList());
    }    
}

