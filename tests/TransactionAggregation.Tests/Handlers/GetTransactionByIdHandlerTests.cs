using NSubstitute;
using TransactionAggregation.Api.API.Responses;
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
        var id = Guid.NewGuid();
        var query = new GetTransactionByIdQuery(id);

        _transactionRepository
            .GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(new AggregatedTransaction());

        await _handler.Handle(query, CancellationToken.None);

        await _transactionRepository.Received(1)
            .GetByIdAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsMappedResponses()
    {
        var id = Guid.NewGuid();
        var query = new GetTransactionByIdQuery(id);

        _transactionRepository
            .GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(new AggregatedTransaction
            {
                Id = id,
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
            });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<AggregatedTransactionResponse>(result);

        Assert.Equal(id, result.Id);
        Assert.Equal("cust-1", result.CustomerId);
        Assert.Equal("BankA", result.Source);
        Assert.Equal("groceries", result.Description);
        Assert.Equal("Checkers", result.Merchant);
        Assert.Equal(-250.75m, result.Amount);
        Assert.Equal("ZAR", result.Currency);
        Assert.Equal("Groceries", result.Category);
    }
}
