namespace ExchangeBot.Exceptions;

public class BadRequestMessageException : Exception
{
    public BadRequestMessageException(string? message) : base(message)
    {
    }
}
