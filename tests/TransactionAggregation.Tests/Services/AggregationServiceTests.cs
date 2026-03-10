using NSubstitute;
using TransactionAggregation.Api.Domain.Entities;
using TransactionAggregation.Api.Domain.External;
using TransactionAggregation.Api.Infrastructure.Repositories;
using TransactionAggregation.Api.Infrastructure.Services;
using Xunit;

namespace TransactionAggregation.Tests.Services;

public class AggregationServiceTests
{
    private readonly ITransactionSourceClient _sourceClient = Substitute.For<ITransactionSourceClient>();
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    private readonly IAggregationRunRepository _aggregationRunRepository = Substitute.For<IAggregationRunRepository>();
    private readonly ITransactionCategorizationService _categorizationService = Substitute.For<ITransactionCategorizationService>();
    private readonly ITransactionIdempotencyService _idempotencyService = Substitute.For<ITransactionIdempotencyService>();

    private AggregationService CreateSut()
    {
        return new AggregationService(
            new[] { _sourceClient },
            _transactionRepository,
            _aggregationRunRepository,
            _categorizationService,
            _idempotencyService);
    }

    [Fact]
    public async Task AggregateAsync_Completes_Run_And_Upserts_Transactions()
    {
        var tx = new SourceTransaction
        {
            SourceName = " BankA ",
            ExternalTransactionId = " tx-1 ",
            CustomerId = " cust-1 ",
            Amount = 100.126m,
            Currency = " zar ",
            Description = " groceries ",
            Merchant = " checkers ",
            TransactionDateUtc = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Unspecified)
        };

        _sourceClient.GetTransactionsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<SourceTransaction> { tx });

        _idempotencyService.Build(Arg.Any<SourceTransaction>())
            .Returns("key-1");

        _categorizationService.Categorize("groceries", "checkers", 100.13m)
            .Returns("Groceries");

        List<AggregatedTransaction>? saved = null;

        _transactionRepository
            .InsertManyIgnoreDuplicatesAsync(
                Arg.Do<List<AggregatedTransaction>>(x => saved = x),
                Arg.Any<CancellationToken>())
            .Returns(1);

        var sut = CreateSut();

        await sut.AggregateAsync("manual");

        Assert.NotNull(saved);
        Assert.Single(saved!);

        var result = saved![0];
        Assert.Equal("BankA", result.Source);
        Assert.Equal("tx-1", result.ExternalTransactionId);
        Assert.Equal("cust-1", result.CustomerId);
        Assert.Equal(100.13m, result.Amount);
        Assert.Equal("ZAR", result.Currency);
        Assert.Equal("groceries", result.Description);
        Assert.Equal("checkers", result.Merchant);
        Assert.Equal("Groceries", result.Category);

        await _aggregationRunRepository.Received(1)
            .UpdateAsync(
                Arg.Is<AggregationRun>(x =>
                    x.Status == Api.Domain.Enums.AggregationRunStatus.Completed &&
                    x.RecordsFetched == 1 &&
                    x.RecordsInserted == 1 &&
                    x.RecordsUpdated == 0 &&
                    x.RecordsSkipped == 0),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AggregateAsync_Dedupes_Transactions_In_Same_Run()
    {
        var tx1 = new SourceTransaction
        {
            SourceName = "BankA",
            ExternalTransactionId = "tx-1",
            CustomerId = "cust-1",
            Amount = 10m,
            Currency = "zar",
            Description = "uber",
            Merchant = "uber",
            TransactionDateUtc = DateTime.UtcNow
        };

        var tx2 = new SourceTransaction
        {
            SourceName = "BankA",
            ExternalTransactionId = "tx-1",
            CustomerId = "cust-1",
            Amount = 10m,
            Currency = "zar",
            Description = "uber",
            Merchant = "uber",
            TransactionDateUtc = DateTime.UtcNow
        };

        _sourceClient.GetTransactionsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<SourceTransaction> { tx1, tx2 });

        _idempotencyService.Build(Arg.Any<SourceTransaction>())
            .Returns("same-key");

        _categorizationService.Categorize(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<decimal>())
            .Returns("Transport");

        List<AggregatedTransaction>? saved = null;

        _transactionRepository
            .InsertManyIgnoreDuplicatesAsync(Arg.Do<List<AggregatedTransaction>>(x => saved = x), Arg.Any<CancellationToken>())
            .Returns(1);

        var sut = CreateSut();

        await sut.AggregateAsync("manual");

        Assert.NotNull(saved);
        Assert.Single(saved!);

        await _aggregationRunRepository.Received(1)
            .UpdateAsync(Arg.Is<AggregationRun>(x =>
                x.Status == Api.Domain.Enums.AggregationRunStatus.Completed &&
                x.RecordsFetched == 2 &&
                x.RecordsInserted == 1 &&
                x.RecordsSkipped == 1), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AggregateAsync_When_Source_Fails_Marks_Run_As_Failed_And_Rethrows()
    {
        _sourceClient.GetTransactionsAsync(Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<List<SourceTransaction>>(new Exception("boom")));

        var sut = CreateSut();

        var ex = await Assert.ThrowsAsync<Exception>(() => sut.AggregateAsync("manual"));

        Assert.Equal("boom", ex.Message);

        await _aggregationRunRepository.Received(1)
            .UpdateAsync(
                Arg.Is<AggregationRun>(x =>
                    x.Status == Api.Domain.Enums.AggregationRunStatus.Failed &&
                    x.ErrorMessage == "boom"),
                Arg.Any<CancellationToken>());

        await _transactionRepository.DidNotReceive()
            .InsertManyIgnoreDuplicatesAsync(
                Arg.Any<List<AggregatedTransaction>>(),
                Arg.Any<CancellationToken>());
    }
}
