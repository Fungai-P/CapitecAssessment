using TransactionAggregation.Api.Domain.Constants;

namespace TransactionAggregation.Api.Infrastructure.Services;

public class TransactionCategorizationService : ITransactionCategorizationService
{
    public string Categorize(string description, string? merchant, decimal amount)
    {
        var text = $"{description} {merchant}".ToLowerInvariant();

        if (amount > 0 && ContainsAny(text, TransactionKeywordConstants.SalaryKeywords))
            return TransactionCategoryConstants.Income;

        if (ContainsAny(text, TransactionKeywordConstants.TransportKeywords))
            return TransactionCategoryConstants.Transport;

        if (ContainsAny(text, TransactionKeywordConstants.GroceryKeywords))
            return TransactionCategoryConstants.Groceries;

        if (ContainsAny(text, TransactionKeywordConstants.EntertainmentKeywords))
            return TransactionCategoryConstants.Entertainment;

        if (ContainsAny(text, TransactionKeywordConstants.FuelKeywords))
            return TransactionCategoryConstants.Fuel;

        if (ContainsAny(text, TransactionKeywordConstants.HousingKeywords))
            return TransactionCategoryConstants.Housing;

        if (ContainsAny(text, TransactionKeywordConstants.HealthcareKeywords))
            return TransactionCategoryConstants.Healthcare;

        if (ContainsAny(text, TransactionKeywordConstants.DiningKeywords))
            return TransactionCategoryConstants.Dining;

        return amount > 0
            ? TransactionCategoryConstants.Income
            : TransactionCategoryConstants.Uncategorized;
    }

    private static bool ContainsAny(string text, IEnumerable<string> keywords)
    {
        foreach (var keyword in keywords)
        {
            if (text.Contains(keyword))
                return true;
        }

        return false;
    }
}