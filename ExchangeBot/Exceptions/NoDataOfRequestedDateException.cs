namespace ExchangeBot.Exceptions;

public class NoDataOfRequestedDateException : Exception
{
    public NoDataOfRequestedDateException(string? message) : base(message)
    {
    }
}
