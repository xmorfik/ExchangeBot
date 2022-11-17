using ExchangeBot.Abstract;
using ExchangeBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Services;

public class Program
{
    private static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection(BotConfiguration.Configuration));

        services.Configure<ApiConfiguration>(
            context.Configuration.GetSection(ApiConfiguration.Configuration));

        services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddSingleton<ICurrencyApiService, CurrencyApiService>();
        services.AddSingleton<IMessageCurrencyLitService, MessageCurrencyLitService>();
        services.AddSingleton<IMessageDateService, MessageDateService>();
        services.AddSingleton<IResponseMessageBuilderService, ResponseMessageBuilderService>();
        services.AddHostedService<PollingService>();
    })
    .Build();

        await host.RunAsync();
    }
}


