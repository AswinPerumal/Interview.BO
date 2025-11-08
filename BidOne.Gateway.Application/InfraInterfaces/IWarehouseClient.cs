using BidOne.Gateway.Domain.Entities;

namespace BidOne.Gateway.Application.InfraInterfaces
{
    public interface IWarehouseClient
    {
        Task<IEnumerable<Stock>> GetStocksAsync();

        Task<IEnumerable<Stock>> GetStockAsync(int productId);
    }
}
