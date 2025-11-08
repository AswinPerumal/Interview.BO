using BidOne.Gateway.Application.InfraInterfaces;
using BidOne.Gateway.Domain.Entities;

namespace BidOne.Gateway.HttpClients
{
    public class ErpClient : IErpClient
    {
        private readonly HttpClient _httpClient;
        public ErpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Mock product data
        public static List<Product> _products = new()
        {
            new() { Id = 1, Name = "iPhone 16", Price = 999 },
            new() { Id = 2, Name = "Samsung S24", Price = 899 },
            new() { Id = 3, Name = "Samsung S25", Price = 1099 },
            new() { Id = 4, Name = "Samsung S25 Ultra", Price = 1199 },
            new() { Id = 5, Name = "iPhone 16 Pro Max", Price = 1299 },
        };

        public Task<IEnumerable<Product>> GetProductsAsync()
        {
            //use _httpClient.GetAsync() here instead of using mock data

            return Task.FromResult<IEnumerable<Product>>(_products);
        }

        public Task<Product> GetProductAsync(int id)
        {
            //use _httpClient.GetAsync() here instead of using mock data

            var product = _products.Where(x => x.Id == id).FirstOrDefault();
            return Task.FromResult<Product>(product);
        }

        public Task<Product> GetProductAsync(string name)
        {
            //use _httpClient.GetAsync() here instead of using mock data

            var product = _products.Where(x => x.Name == name).FirstOrDefault();
            return Task.FromResult<Product>(product);
        }

        public Task<Product> UpdateProductAsync(Product product)
        {
            //use _httpClient.PutAsync() here instead of using mock data

            var index = _products.FindIndex(x => x.Id == product.Id);

            if (index >= 0)
            {
                _products[index] = product;
                return Task.FromResult(product);
            }

            return null;
        }

        public Task<Product> AddProductAsync(Product product)
        {
            //use _httpClient.PostAsync() here instead of using mock data

            _products.Add(product);

            return Task.FromResult<Product>(_products.FirstOrDefault(x => x.Id == product.Id));
        }

        public Task<int> DeleteProductAsync(int id)
        {
            //use _httpClient.DeleteAsync() here instead of using mock data

            var product = _products.Where(x => x.Id == id).FirstOrDefault();

            _products.Remove(product);

            return Task.FromResult(1);
        }
    }
}
