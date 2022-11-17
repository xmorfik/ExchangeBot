using ExchangeBot.Abstract;
using ExchangeBot.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ExchangeBot.Services;

public class ResponseMessageBuilderService : IResponseMessageBuilderService
{
    private readonly IMessageDateService _messageDateService;
    private readonly IMessageCurrencyLitService _currencyLitService;
    private readonly ICurrencyApiService _currencyApiService;
    private readonly ILogger _logger;

    public ResponseMessageBuilderService(
        IMessageCurrencyLitService lit, 
        IMessageDateService date,
        ICurrencyApiService api,
        ILogger<ResponseMessageBuilderService> logger)
    {
        _messageDateService = date;
        _currencyLitService = lit;
        _currencyApiService = api;
        _logger = logger;
    }

    public async Task<string> GetMessageAsync(string reqest)
    {
        _logger.LogInformation("Message text: {0}", reqest);

        var messageLit = _currencyLitService.GetLit(reqest);
        var messageDate = _messageDateService.GetDate(reqest);
        var apiResponse = await _currencyApiService.GetCurrencyAsync(messageDate);

        if(apiResponse.ExchangeRate.Count== 0)
        {
            throw new NoDataOfRequestedDateException("No data of requested date");
        }

        var exchangeRate = apiResponse.ExchangeRate.First(er => er.Currency == messageLit);

        var result = new StringBuilder();
        result.Append("Exchange rate of " + messageLit.ToString()+ " to " + apiResponse.BaseCurrencyLit.ToString());
        result.Append("\nFor " + messageDate.ToShortDateString()+"\n");
        result.Append(exchangeRate.PurchaseRateNB.ToString());
        result.Append("/");
        result.Append(exchangeRate.SaleRateNB.ToString());

        return result.ToString();
    }
}
