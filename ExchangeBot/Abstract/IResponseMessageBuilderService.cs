namespace ExchangeBot.Abstract;

public interface IResponseMessageBuilderService
{
    public Task<string> GetMessageAsync(string request);
}
