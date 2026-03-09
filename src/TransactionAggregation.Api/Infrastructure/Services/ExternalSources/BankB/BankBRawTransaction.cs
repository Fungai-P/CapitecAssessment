namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.BankB;

public class BankBRawTransaction
{
    public string TxId { get; set; } = "";
    public string Customer { get; set; } = "";
    public decimal Amount { get; set; }
    public string Merchant { get; set; } = "";
    public long UnixTimestamp { get; set; }
}
