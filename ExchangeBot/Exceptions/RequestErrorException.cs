namespace ExchangeBot.Exceptions;

public class RequestErrorException : Exception
{
    public RequestErrorException(string? message) : base(message)
    {
    }
}
