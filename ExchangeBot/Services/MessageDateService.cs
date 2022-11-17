using ExchangeBot.Abstract;
using ExchangeBot.Exceptions;

namespace ExchangeBot.Services;

public class MessageDateService : IMessageDateService
{
    private const int MessageSize = 14;
    private const int MessageRank = 1;

    public DateTime GetDate(string message)
    {
        var trimMessage = message.Trim();
        if (trimMessage.Length != MessageSize)
        {
            throw new BadRequestMessageException("Invalid format");
        }

        var request = trimMessage.Split(' ');
        if (request.Rank != MessageRank)
        {
            throw new BadRequestMessageException("Invalid format");
        }

        try
        {
            var date = DateTime.Parse(request[1]);
            return date;
        }
        catch(Exception)
        {
            throw new InvalidDateException(string.Format("Unknown date {0}", request[1]));
        }
    }
}
