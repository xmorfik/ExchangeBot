namespace ExchangeBot.Model;

public class ExchangeCurrency
{
    public string BaseCurrency = string.Empty;
    public CurrencyLit Currency { get; set; }
    public double SaleRateNB { get; set; }
    public double PurchaseRateNB { get; set; }
}
