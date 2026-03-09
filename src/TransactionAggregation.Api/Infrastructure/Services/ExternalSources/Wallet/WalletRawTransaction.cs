namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.Wallet;

public class WalletRawTransaction
{
    public string WalletTx { get; set; } = "";
    public string User { get; set; } = "";
    public decimal Value { get; set; }
    public string Description { get; set; } = "";
    public string Timestamp { get; set; } = "";
}
