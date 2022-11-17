using ExchangeBot.Model;

namespace ExchangeBot.Abstract;

public interface IMessageCurrencyLitService
{
    public CurrencyLit GetLit(string message);
}
