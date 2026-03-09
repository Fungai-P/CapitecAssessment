using NSubstitute;
using TransactionAggregation.Api.API.Responses;
using TransactionAggregation.Api.Application.Handlers;
using TransactionAggregation.Api.Domain.Entities;
using TransactionAggregation.Api.Domain.Models;
using TransactionAggregation.Api.Infrastructure.Repositories;
using TransactionAggregation.Api.Models.Requests;
using Xunit;

namespace TransactionAggregation.Tests.Handlers;

public class SearchTransactionHandlerTests
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly SearchTransactionHandler _handler;

    public SearchTransactionHandlerTests()
    {
        _transactionRepository = Substitute.For<ITransactionRepository>();
        _handler = new SearchTransactionHandler(_transactionRepository);
    }

    [Fact]
    public async Task Handle_CallsRepositorySearchAsync()
    {
        var command = new TransactionSearchCommand
        {
            CustomerId = "cust-1",
            Category = "Groceries",
            Source = "BankA",
            Page = 2,
            PageSize = 10
        };

        var repositoryResult = new PagedResult<AggregatedTransaction>
        {
            Items = new List<AggregatedTransaction>(),
            TotalCount = 0,
            Page = 2,
            PageSize = 10
        };

        _transactionRepository
            .SearchAsync(Arg.Any<TransactionSearch>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(repositoryResult);

        await _handler.Handle(command, CancellationToken.None);

        await _transactionRepository.Received(1)
            .SearchAsync(Arg.Any<TransactionSearch>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsMappedPagedResult()
    {
        var command = new TransactionSearchCommand
        {
            CustomerId = "cust-1",
            Page = 1,
            PageSize = 20
        };

        var repositoryResult = new PagedResult<AggregatedTransaction>
        {
            Items = new List<AggregatedTransaction>
            {
                new AggregatedTransaction
                {
                    CustomerId = "cust-1",
                    Category = "Groceries",
                    Source = "BankA",
                    Amount = 150.25m
                }
            },
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };

        _transactionRepository
            .SearchAsync(Arg.Any<TransactionSearch>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(repositoryResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Single(result.Items);
        Assert.Equal("cust-1", result.Items.FirstOrDefault()!.CustomerId);
        Assert.Equal("Groceries", result.Items.FirstOrDefault()!.Category);
        Assert.Equal("BankA", result.Items.FirstOrDefault()!.Source);
        Assert.Equal(150.25m, result.Items.FirstOrDefault()!.Amount);
    }
}
