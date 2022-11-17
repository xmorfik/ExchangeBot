namespace ExchangeBot.Model;

public class BaseCurrency
{
    public CurrencyLit BaseCurrencyLit { get; set; }
    public DateTime Date;
    public List<ExchangeCurrency> ExchangeRate = new();
}