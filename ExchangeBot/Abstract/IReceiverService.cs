namespace Telegram.Bot.Abstract;

public interface IReceiverService
{
    public Task ReceiveAsync(CancellationToken stoppingToken);
}
