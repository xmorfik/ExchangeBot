using ExchangeBot.Model;

namespace ExchangeBot.Abstract;

public interface ICurrencyApiService
{
    public Task<BaseCurrency> GetCurrencyAsync(DateTime date);
}
