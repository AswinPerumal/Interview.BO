using BidOne.Gateway.Domain.Entities;

namespace BidOne.Gateway.Application.Interfaces
{
    public interface IProductService
    {
        Task<Product> CreateOrUpdateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetProductAsync(int id);
        Task<int> DeleteProductAsync(int id);
    }
}
