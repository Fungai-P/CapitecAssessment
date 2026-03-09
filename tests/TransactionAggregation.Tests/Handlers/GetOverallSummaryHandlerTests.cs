using NSubstitute;
using TransactionAggregation.Api.Application.Handlers;
using TransactionAggregation.Api.Application.Queries;
using TransactionAggregation.Api.Domain.Models;
using TransactionAggregation.Api.Infrastructure.Repositories;
using Xunit;

namespace TransactionAggregation.Tests.Handlers;

public class GetOverallSummaryHandlerTests
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly GetOverallSummaryHandler _handler;

    public GetOverallSummaryHandlerTests()
    {
        _transactionRepository = Substitute.For<ITransactionRepository>();
        _handler = new GetOverallSummaryHandler(_transactionRepository);
    }

    [Fact]
    public async Task Handle_CallsRepository()
    {
        var query = new GetOverallSummaryQuery();

        _transactionRepository
            .GetOverallSummaryAsync(Arg.Any<CancellationToken>())
            .Returns(new TransactionSummary
            {
                TotalCredits = 0,
                TotalDebits = 0,
                TransactionCount = 0,
                NetAmount = 0
            });

        await _handler.Handle(query, CancellationToken.None);

        await _transactionRepository.Received(1)
            .GetOverallSummaryAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsMappedSummary()
    {
        var query = new GetOverallSummaryQuery();

        _transactionRepository
            .GetOverallSummaryAsync(Arg.Any<CancellationToken>())
            .Returns(new TransactionSummary
            {
                TotalCredits = 12000.50m,
                TotalDebits = 4500.25m,
                TransactionCount = 15,
                NetAmount = 7500.25m
            });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(15, result.TransactionCount);
        Assert.Equal(12000.50m, result.TotalCredits);
        Assert.Equal(4500.25m, result.TotalDebits);
        Assert.Equal(7500.25m, result.NetAmount);
    }
}
