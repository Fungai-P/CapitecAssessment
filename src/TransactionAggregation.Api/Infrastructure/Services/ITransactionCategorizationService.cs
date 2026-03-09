namespace TransactionAggregation.Api.Infrastructure.Services;

public interface ITransactionCategorizationService
{
    string Categorize(string description, string? merchant, decimal amount);
}
