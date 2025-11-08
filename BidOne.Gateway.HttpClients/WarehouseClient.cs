using BidOne.Gateway.Application.InfraInterfaces;
using BidOne.Gateway.Domain.Entities;

namespace BidOne.Gateway.HttpClients
{
    public class WarehouseClient : IWarehouseClient
    {
        private readonly HttpClient _httpClient;

        // Mock stock data
        public static List<Stock> _stocks = new()
       {
            new() { Id = 1, Count = 112, ProductId = 1, WarehouseAddress = "Albany, Auckland" },
            new() { Id = 2, Count = 456, ProductId = 1, WarehouseAddress = "Rosedale, Auckland" },
             new() { Id = 3, Count = 753, ProductId = 2, WarehouseAddress = "Rosedale, Auckland" },
              new() { Id = 4, Count = 259, ProductId = 1, WarehouseAddress = "CBD, Auckland" },
               new() { Id = 5, Count = 357, ProductId = 2, WarehouseAddress = "Albany, Auckland" },
        };

        public WarehouseClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<IEnumerable<Stock>> GetStockAsync(int productId)
        {
            //use _httpClient.GetAsync() here instead of using mock data

            var stock = _stocks.Where(x => x.ProductId == productId);
            return Task.FromResult(stock);
        }

        public Task<IEnumerable<Stock>> GetStocksAsync()
        {   
            //use _httpClient.GetAsync() here instead of using mock data

            return Task.FromResult<IEnumerable<Stock>>(_stocks);
        }
    }
}
