namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.BankA;

public class BankARawTransaction
{
    public string Id { get; set; } = "";
    public string AccountHolder { get; set; } = "";
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string MerchantName { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime DateUtc { get; set; }
}

