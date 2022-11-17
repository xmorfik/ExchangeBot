using ExchangeBot.Abstract;
using ExchangeBot.Model;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly IResponseMessageBuilderService _responseBuilder;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(
        ITelegramBotClient botClient, 
        ILogger<UpdateHandler> logger, 
        IResponseMessageBuilderService responseBuilder)
    {
        _botClient = botClient;
        _logger = logger;
        _responseBuilder = responseBuilder;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            { Message: { } message }       => BotOnMessageReceived(message, cancellationToken),
            _                              => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Text is not { } messageText)
            return;

        var action = messageText.Split(' ')[0] switch
        {
            "/start"           => Start(_botClient, message, cancellationToken),
            "/help"            => Help(_botClient, message, cancellationToken),
            "/currency"        => GetCurrencyList(_botClient, message, cancellationToken),
            _                  => Request(_botClient, message, cancellationToken)
        };

        Message sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

        async Task<Message> Request(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var response = string.Empty;
            try
            {
                response = await _responseBuilder.GetMessageAsync(message.Text);
            }
            catch(Exception ex)
            {
                response = ex.Message;
            }

            _logger.LogInformation("Response text: {0}", response);

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: response,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        async Task<Message> GetCurrencyList(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var currencyList = Enum.GetNames<CurrencyLit>();
           
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: string.Join("\n", currencyList),
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        async Task<Message> Help(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            const string example = "USD 11.11.2022";

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: example,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        async Task<Message> Start(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            const string usage = "Usage:\n" +
                                 "USD DD.MM.YYYY - send exchange\n" +
                                 "/currency - send available currency\n" +
                                 "/help - send example\n";

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }
    }

    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}
