namespace BidOne.Gateway.Application.Interfaces
{
    public interface IIdempotencyService
    {
        Task<(bool IsProcessing, object? Response, int StatusCode)?> GetStoredResponseAsync(string key, string method, string hash);
        Task MarkProcessingAsync(string key, string method, string hash);
        Task SaveResponseAsync(string key, string method, string hash, object? response, int statusCode);
    }
}
