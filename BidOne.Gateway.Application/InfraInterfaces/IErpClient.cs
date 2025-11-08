using BidOne.Gateway.Domain.Entities;

namespace BidOne.Gateway.Application.InfraInterfaces
{
    public interface IErpClient
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int id);
        Task<Product> GetProductAsync(string name);
        Task<Product> UpdateProductAsync(Product product);
        Task<Product> AddProductAsync(Product product);
        Task<int> DeleteProductAsync(int id);
    }
}
