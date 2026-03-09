namespace TransactionAggregation.Api.Infrastructure.Services;

public class TransactionCategorizationService : ITransactionCategorizationService
{
    public string Categorize(string description, string? merchant, decimal amount)
    {
        var text = $"{description} {merchant}".ToLowerInvariant();

        if (amount > 0 && (text.Contains("salary") || text.Contains("wage")))
            return "Income";

        if (text.Contains("uber") || text.Contains("bolt"))
            return "Transport";

        if (text.Contains("shoprite") || text.Contains("checkers") || text.Contains("pick n pay") || text.Contains("woolworths"))
            return "Groceries";

        if (text.Contains("netflix") || text.Contains("spotify") || text.Contains("showmax") || text.Contains("dstv"))
            return "Entertainment";

        if (text.Contains("shell") || text.Contains("engen") || text.Contains("bp") || text.Contains("sasol"))
            return "Fuel";

        if (text.Contains("rent") || text.Contains("landlord") || text.Contains("property"))
            return "Housing";

        if (text.Contains("pharmacy") || text.Contains("clinic") || text.Contains("hospital") || text.Contains("medical"))
            return "Healthcare";

        if (text.Contains("restaurant") || text.Contains("kfc") || text.Contains("mcd") || text.Contains("steers") || text.Contains("debonairs"))
            return "Dining";

        return amount > 0 ? "Income" : "Uncategorized";
    }
}
