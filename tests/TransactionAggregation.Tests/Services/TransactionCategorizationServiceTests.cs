using TransactionAggregation.Api.Domain.Constants;
using TransactionAggregation.Api.Infrastructure.Services;
using Xunit;

namespace TransactionAggregation.Tests.Services;

public class TransactionCategorizationServiceTests
{
    private readonly TransactionCategorizationService _service = new();

    [Theory]
    [InlineData("monthly salary payment", null, 5000, TransactionCategoryConstants.Income)]
    [InlineData("company payroll", null, 2500, TransactionCategoryConstants.Income)]
    [InlineData("weekly wage", null, 900, TransactionCategoryConstants.Income)]
    [InlineData("uber trip", null, -120, TransactionCategoryConstants.Transport)]
    [InlineData("checkers xtra savings", null, -250, TransactionCategoryConstants.Groceries)]
    [InlineData("netflix subscription", null, -199, TransactionCategoryConstants.Entertainment)]
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

        Assert.Equal(TransactionCategoryConstants.Transport, result);
    }

    [Fact]
    public void Categorize_IsCaseInsensitive()
    {
        var result = _service.Categorize("NETFLIX subscription", null, -199);

        Assert.Equal(TransactionCategoryConstants.Entertainment, result);
    }

    [Fact]
    public void Categorize_UnknownPositiveAmount_ReturnsIncome()
    {
        var result = _service.Categorize("random transfer", null, 100);

        Assert.Equal(TransactionCategoryConstants.Income, result);
    }

    [Fact]
    public void Categorize_UnknownNegativeAmount_ReturnsUncategorized()
    {
        var result = _service.Categorize("random card purchase", null, -100);

        Assert.Equal(TransactionCategoryConstants.Uncategorized, result);
    }

    [Fact]
    public void Categorize_SalaryKeyword_WithNegativeAmount_DoesNotReturnIncomeKeywordRule()
    {
        var result = _service.Categorize("salary reversal", null, -5000);

        Assert.Equal(TransactionCategoryConstants.Uncategorized, result);
    }
}