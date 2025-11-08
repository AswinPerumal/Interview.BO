namespace BidOne.Gateway.Application.InfraInterfaces
{
    public interface ICacheService
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? ttl = null);
        void Remove(string key);
    }
}
