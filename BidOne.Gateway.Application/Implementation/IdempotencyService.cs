using BidOne.Gateway.Application.Interfaces;
using System.Collections.Concurrent;

namespace BidOne.Gateway.Application.Implementation
{
    public class IdempotencyService : IIdempotencyService
    {
        private class Entry
        {
            public bool IsProcessing { get; set; }
            public object? Response { get; set; }
            public int StatusCode { get; set; }
        }


        private static readonly ConcurrentDictionary<string, Entry> Store = new();


        private static string BuildKey(string key, string method, string hash)
        => $"{key}:{method}:{hash}";


        public Task<(bool IsProcessing, object? Response, int StatusCode)?> GetStoredResponseAsync(string key, string method, string hash)
        {
            var compositeKey = BuildKey(key, method, hash);
            if (Store.TryGetValue(compositeKey, out var entry))
            {
                return Task.FromResult<(bool, object?, int)?>((entry.IsProcessing, entry.Response, entry.StatusCode));
            }
            return Task.FromResult<(bool, object?, int)?>(null);
        }


        public Task MarkProcessingAsync(string key, string method, string hash)
        {
            var compositeKey = BuildKey(key, method, hash);
            Store[compositeKey] = new Entry { IsProcessing = true };
            return Task.CompletedTask;
        }

        public Task SaveResponseAsync(string key, string method, string hash, object? response, int statusCode)
        {
            var compositeKey = BuildKey(key, method, hash);
            Store[compositeKey] = new Entry { IsProcessing = false, Response = response, StatusCode = statusCode };
            return Task.CompletedTask;
        }

        public Task RemoveKeyAsync(string key, string method, string hash)
        {
            var compositeKey = BuildKey(key, method, hash);
            Store.TryRemove(compositeKey, out _);
            return Task.CompletedTask;
        }
    }
}
