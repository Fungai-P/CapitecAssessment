using NSubstitute;
using TransactionAggregation.Api.Application.Handlers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Domain.Entities;
using TransactionAggregation.Api.Infrastructure.Repositories;
using Xunit;

namespace TransactionAggregation.Tests.Handlers;

public class GetCustomerTransactionsHandlerTests
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly GetCustomerTransactionsHandler _handler;

    public GetCustomerTransactionsHandlerTests()
    {
        _transactionRepository = Substitute.For<ITransactionRepository>();
        _handler = new GetCustomerTransactionsHandler(_transactionRepository);
    }

    [Fact]
    public async Task Handle_CallsRepository_WithCustomerId()
    {
        var query = new GetCustomerTransactionsQuery("cust-1");

        _transactionRepository
            .GetByCustomerAsync("cust-1", Arg.Any<CancellationToken>())
            .Returns(new List<AggregatedTransaction>());

        await _handler.Handle(query, CancellationToken.None);

        await _transactionRepository.Received(1)
            .GetByCustomerAsync("cust-1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsMappedTransactions()
    {
        var query = new GetCustomerTransactionsQuery("cust-1");

        _transactionRepository
            .GetByCustomerAsync("cust-1", Arg.Any<CancellationToken>())
            .Returns(new List<AggregatedTransaction>
            {
                new AggregatedTransaction
                {
                    Id = Guid.NewGuid(),
                    CustomerId = "cust-1",
                    ExternalTransactionId = "tx-1",
                    Source = "BankA",
                    Description = "groceries",
                    Merchant = "Checkers",
                    Amount = -200,
                    Currency = "ZAR",
                    Category = "Groceries",
                    TransactionDateUtc = DateTime.UtcNow,
                    CreatedAtUtc = DateTime.UtcNow,
                    IdempotencyKey = "key-1"
                }
            });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result);

        var transaction = result.First();

        Assert.Equal("cust-1", transaction.CustomerId);
        Assert.Equal("BankA", transaction.Source);
        Assert.Equal("groceries", transaction.Description);
        Assert.Equal("Checkers", transaction.Merchant);
        Assert.Equal(-200, transaction.Amount);
        Assert.Equal("ZAR", transaction.Currency);
        Assert.Equal("Groceries", transaction.Category);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyCollection_WhenNoTransactions()
    {
        var query = new GetCustomerTransactionsQuery("cust-1");

        _transactionRepository
            .GetByCustomerAsync("cust-1", Arg.Any<CancellationToken>())
            .Returns(new List<AggregatedTransaction>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}
