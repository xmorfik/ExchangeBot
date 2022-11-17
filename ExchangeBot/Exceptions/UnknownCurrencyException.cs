namespace ExchangeBot.Exceptions;

public class UnknownCurrencyException : Exception
{
    public UnknownCurrencyException(string? message) : base(message)
    {
    }
}
