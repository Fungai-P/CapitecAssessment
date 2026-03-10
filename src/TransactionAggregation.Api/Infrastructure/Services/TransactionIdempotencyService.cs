using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TransactionAggregation.Api.Domain.External;

namespace TransactionAggregation.Api.Infrastructure.Services;

public class TransactionIdempotencyService : ITransactionIdempotencyService
{
    public string Build(SourceTransaction transaction)
    {
        var merchant = NormalizeText(transaction.Merchant);
        var description = NormalizeText(transaction.Description);

        // Simmilar transactions within the minute will be merged together, to avoid minor differences in the transaction date to create different fingerprints.
        var minuteBucket = new DateTime(
            transaction.TransactionDateUtc.Year,
            transaction.TransactionDateUtc.Month,
            transaction.TransactionDateUtc.Day,
            transaction.TransactionDateUtc.Hour,
            transaction.TransactionDateUtc.Minute,
            0,
            DateTimeKind.Utc);

        var raw = string.Join("|",
            transaction.CustomerId.Trim().ToLowerInvariant(),
            Math.Round(transaction.Amount, 2).ToString("0.00"),
            merchant,
            description,
            minuteBucket.ToString("yyyy-MM-ddTHH:mm:ssZ"));

        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes);
    }

    private static string NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim().ToLowerInvariant();

        normalized = normalized.Replace("-", "");
        normalized = normalized.Replace("_", "");
        normalized = normalized.Replace(".", "");

        normalized = Regex.Replace(normalized, @"\s+", " ");

        return normalized;
    }
}
