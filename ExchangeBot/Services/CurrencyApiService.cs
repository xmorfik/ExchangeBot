using ExchangeBot.Abstract;
using ExchangeBot.Exceptions;
using ExchangeBot.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExchangeBot.Services;

public class CurrencyApiService : ICurrencyApiService
{
    private readonly IOptions<ApiConfiguration> _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public CurrencyApiService(IOptions<ApiConfiguration> options, ILogger<CurrencyApiService> logger)
    {
        _options = options;
        _logger = logger;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(_options.Value.Uri)
        };
    }

    public async Task<BaseCurrency> GetCurrencyAsync(DateTime date)
    {
        var url = string.Format(_options.Value.Part + "{0}", date.ToShortDateString());
        var response = await _httpClient.GetAsync(url);
        var stringResponse = string.Empty;

        _logger.LogInformation(response?.RequestMessage?.ToString());

        if (response.IsSuccessStatusCode)
        {
            stringResponse = await response.Content.ReadAsStringAsync();
        }
        else
        {
            throw new RequestErrorException("Server error, bad request");
        }

        try
        {
            var result = JsonConvert.DeserializeObject<BaseCurrency>(stringResponse, new IsoDateTimeConverter { DateTimeFormat = "dd.MM.yyyy" });
            return result;
        }
        catch(Exception)
        {
            throw new UnableConvertException("Fail to convert response from PrivatBank");
        }
    }
}
