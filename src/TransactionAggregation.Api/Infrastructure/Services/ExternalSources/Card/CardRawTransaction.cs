namespace TransactionAggregation.Api.Infrastructure.Services.ExternalSources.Card;

public class CardRawTransaction
{
    public string CardId { get; set; } = "";
    public string CustomerRef { get; set; } = "";
    public decimal Value { get; set; }
    public string Vendor { get; set; } = "";
    public DateTime ProcessedAt { get; set; }
}
