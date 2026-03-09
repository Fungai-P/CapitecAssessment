using NSubstitute;
using TransactionAggregation.Api.Application.Handlers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Domain.Entities;
using TransactionAggregation.Api.Infrastructure.Repositories;
using Xunit;

namespace TransactionAggregation.Tests.Handlers;

public class GetTransactionByIdHandlerTests
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly GetTransactionByIdHandler _handler;

    public GetTransactionByIdHandlerTests()
    {
        _transactionRepository = Substitute.For<ITransactionRepository>();
        _handler = new GetTransactionByIdHandler(_transactionRepository);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCustomerId()
    {
        var query = new GetTransactionByIdQuery("cust-1");

        _transactionRepository
            .GetByCustomerAsync("cust-1", Arg.Any<CancellationToken>())
            .Returns(new List<AggregatedTransaction>());

        await _handler.Handle(query, CancellationToken.None);

        await _transactionRepository.Received(1)
            .GetByCustomerAsync("cust-1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsMappedResponses()
    {
        var query = new GetTransactionByIdQuery("cust-1");

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
                    Amount = -250.75m,
                    Currency = "ZAR",
                    Category = "Groceries",
                    TransactionDateUtc = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc),
                    CreatedAtUtc = new DateTime(2026, 3, 1, 10, 5, 0, DateTimeKind.Utc),
                    IdempotencyKey = "key-1"
                }
            });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);

        var item = result.First();
        Assert.Equal("cust-1", item.CustomerId);
        Assert.Equal("BankA", item.Source);
        Assert.Equal("groceries", item.Description);
        Assert.Equal("Checkers", item.Merchant);
        Assert.Equal(-250.75m, item.Amount);
        Assert.Equal("ZAR", item.Currency);
        Assert.Equal("Groceries", item.Category);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyCollection_WhenRepositoryReturnsNone()
    {
        var query = new GetTransactionByIdQuery("cust-1");

        _transactionRepository
            .GetByCustomerAsync("cust-1", Arg.Any<CancellationToken>())
            .Returns(new List<AggregatedTransaction>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
