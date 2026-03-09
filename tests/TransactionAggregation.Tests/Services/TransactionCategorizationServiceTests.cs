using TransactionAggregation.Api.Infrastructure.Services;
using Xunit;

namespace TransactionAggregation.Tests.Services;

public class TransactionCategorizationServiceTests
{
    private readonly TransactionCategorizationService _service = new();

    [Theory]
    [InlineData("monthly salary payment", null, 5000, "Income")]
    [InlineData("company payroll", null, 2500, "Income")]
    [InlineData("weekly wage", null, 900, "Income")]
    [InlineData("uber trip", null, -120, "Transport")]
    [InlineData("checkers xtra savings", null, -250, "Groceries")]
    [InlineData("netflix subscription", null, -199, "Entertainment")]

    public void Categorize_KnownKeywords_ReturnsExpectedCategory(
        string description,
        string? merchant,
        decimal amount,
        string expectedCategory)
    {
        var result = _service.Categorize(description, merchant, amount);

        Assert.Equal(expectedCategory, result);
    }

    [Fact]
    public void Categorize_UsesMerchantText_AsWell()
    {
        var result = _service.Categorize("card purchase", "Uber", -50);

        Assert.Equal("Transport", result);
    }

    [Fact]
    public void Categorize_IsCaseInsensitive()
    {
        var result = _service.Categorize("NETFLIX subscription", null, -199);

        Assert.Equal("Entertainment", result);
    }

    [Fact]
    public void Categorize_UnknownPositiveAmount_ReturnsIncome()
    {
        var result = _service.Categorize("random transfer", null, 100);

        Assert.Equal("Income", result);
    }

    [Fact]
    public void Categorize_UnknownNegativeAmount_ReturnsUncategorized()
    {
        var result = _service.Categorize("random card purchase", null, -100);

        Assert.Equal("Uncategorized", result);
    }

    [Fact]
    public void Categorize_SalaryKeyword_WithNegativeAmount_DoesNotReturnIncomeKeywordRule()
    {
        var result = _service.Categorize("salary reversal", null, -5000);

        Assert.Equal("Uncategorized", result);
    }
}
