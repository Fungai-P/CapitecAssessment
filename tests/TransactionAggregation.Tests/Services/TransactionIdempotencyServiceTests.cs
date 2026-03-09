using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionAggregation.Api.Domain.External;
using TransactionAggregation.Api.Infrastructure.Services;
using Xunit;

namespace TransactionAggregation.Tests.Services;

public class TransactionIdempotencyServiceTests
{
    private readonly TransactionIdempotencyService _service = new();

    [Fact]
    public void Build_SameTransaction_ReturnsSameKey()
    {
        var tx = CreateTransaction();

        var key1 = _service.Build(tx);
        var key2 = _service.Build(tx);

        Assert.Equal(key1, key2);
    }

    [Fact]
    public void Build_Ignores_TextFormattingDifferences()
    {
        var tx1 = CreateTransaction(
            merchant: "Checkers-Hyper",
            description: " Grocery Store "
        );

        var tx2 = CreateTransaction(
            merchant: "checkershyper",
            description: "grocery store"
        );

        var key1 = _service.Build(tx1);
        var key2 = _service.Build(tx2);

        Assert.Equal(key1, key2);
    }

    [Fact]
    public void Build_Rounds_Amount_ToTwoDecimals()
    {
        var tx1 = CreateTransaction(amount: 100.124m);
        var tx2 = CreateTransaction(amount: 100.12m);

        var key1 = _service.Build(tx1);
        var key2 = _service.Build(tx2);

        Assert.Equal(key1, key2);
    }

    [Fact]
    public void Build_Uses_MinuteBucket_ForTransactionDate()
    {
        var tx1 = CreateTransaction(date: new DateTime(2026, 3, 1, 10, 15, 05, DateTimeKind.Utc));
        var tx2 = CreateTransaction(date: new DateTime(2026, 3, 1, 10, 15, 55, DateTimeKind.Utc));

        var key1 = _service.Build(tx1);
        var key2 = _service.Build(tx2);

        Assert.Equal(key1, key2);
    }

    [Fact]
    public void Build_DifferentMinute_ReturnsDifferentKey()
    {
        var tx1 = CreateTransaction(date: new DateTime(2026, 3, 1, 10, 15, 59, DateTimeKind.Utc));
        var tx2 = CreateTransaction(date: new DateTime(2026, 3, 1, 10, 16, 00, DateTimeKind.Utc));

        var key1 = _service.Build(tx1);
        var key2 = _service.Build(tx2);

        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public void Build_Normalizes_CustomerId_WithTrimAndLowercase()
    {
        var tx1 = CreateTransaction(customerId: " Cust-001 ");
        var tx2 = CreateTransaction(customerId: "cust-001");

        var key1 = _service.Build(tx1);
        var key2 = _service.Build(tx2);

        Assert.Equal(key1, key2);
    }

    [Fact]
    public void Build_NullMerchantAndDescription_AreHandled()
    {
        var tx1 = CreateTransaction(merchant: null, description: null);
        var tx2 = CreateTransaction(merchant: "", description: "");

        var key1 = _service.Build(tx1);
        var key2 = _service.Build(tx2);

        Assert.Equal(key1, key2);
    }

    private static SourceTransaction CreateTransaction(
        string customerId = "cust-1",
        decimal amount = 100.12m,
        string? merchant = "Checkers",
        string? description = "Groceries",
        DateTime? date = null)
    {
        return new SourceTransaction
        {
            CustomerId = customerId,
            Amount = amount,
            Merchant = merchant,
            Description = description,
            TransactionDateUtc = date ?? new DateTime(2026, 3, 1, 10, 15, 30, DateTimeKind.Utc)
        };
    }
}
