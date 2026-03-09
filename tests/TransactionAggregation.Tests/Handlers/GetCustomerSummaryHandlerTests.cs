using NSubstitute;
using TransactionAggregation.Api.Application.Handlers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Domain.Models;
using TransactionAggregation.Api.Infrastructure.Repositories;
using Xunit;

namespace TransactionAggregation.Tests.Handlers;

public class GetCustomerSummaryHandlerTests
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly GetCustomerSummaryHandler _handler;

    public GetCustomerSummaryHandlerTests()
    {
        _transactionRepository = Substitute.For<ITransactionRepository>();
        _handler = new GetCustomerSummaryHandler(_transactionRepository);
    }

    [Fact]
    public async Task Handle_CallsRepository_WithCustomerId()
    {
        var query = new GetCustomerSummaryQuery("cust-1");

        _transactionRepository
            .GetCustomerSummaryAsync("cust-1", Arg.Any<CancellationToken>())
            .Returns(new CustomerSummary
            {
                CustomerId = "cust-1",
                TotalCredits = 0,
                TotalDebits = 0,
                TransactionCount = 0,
                NetAmount = 0
            });

        await _handler.Handle(query, CancellationToken.None);

        await _transactionRepository.Received(1)
            .GetCustomerSummaryAsync("cust-1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsMappedSummary()
    {
        var query = new GetCustomerSummaryQuery("cust-1");

        _transactionRepository
            .GetCustomerSummaryAsync("cust-1", Arg.Any<CancellationToken>())
            .Returns(new CustomerSummary
            {
                CustomerId = "cust-1",
                TransactionCount = 12,
                TotalCredits = 15000m,
                TotalDebits = 4200m,
                NetAmount = 10800m
            });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("cust-1", result.CustomerId);
        Assert.Equal(12, result.TransactionCount);
        Assert.Equal(15000m, result.TotalCredits);
        Assert.Equal(4200m, result.TotalDebits);
        Assert.Equal(10800m, result.NetAmount);
    }
}
