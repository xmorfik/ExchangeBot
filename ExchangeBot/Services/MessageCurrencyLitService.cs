using ExchangeBot.Abstract;
using ExchangeBot.Exceptions;
using ExchangeBot.Model;

namespace ExchangeBot.Services;

public class MessageCurrencyLitService : IMessageCurrencyLitService
{
    private const int MessageSize = 14;
    private const int MessageRank = 1;

    public CurrencyLit GetLit(string message)
    {
        var trimMessage = message.Trim();
        if(trimMessage.Length != MessageSize)
        {
            throw new BadRequestMessageException("Invalid format");
        }

        var request = trimMessage.Split(' ');
        if(request.Rank != MessageRank)
        {
            throw new BadRequestMessageException("Invalid format");
        }

        try
        {
            var lit = Enum.Parse<CurrencyLit>(request[0]);
            return lit;
        }
        catch(Exception)
        {
            throw new UnknownCurrencyException(string.Format("Unknown currency {0}", request[0]));
        }
    }
}
