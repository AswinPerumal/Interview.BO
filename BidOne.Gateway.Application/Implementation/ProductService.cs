using BidOne.Gateway.Application.InfraInterfaces;
using BidOne.Gateway.Application.Interfaces;
using BidOne.Gateway.Domain.Constants;
using BidOne.Gateway.Domain.Entities;
using BidOne.Gateway.Domain.Enums;
using BidOne.Gateway.Domain.Exceptions;

namespace BidOne.Gateway.Application.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IErpClient _erpClient;
        private readonly IWarehouseClient _warehouseClient;
        private readonly ICacheService _cacheService;

        public ProductService(IErpClient erpClient, IWarehouseClient warehouseClient, ICacheService cacheService)
        {
            _erpClient = erpClient;
            _warehouseClient = warehouseClient;
            _cacheService = cacheService;
        }

        public async Task<Product> CreateOrUpdateProductAsync(Product product)
        {
            if (product.Id == default)
            {
                return await _erpClient.AddProductAsync(product);
            }

            var existingProduct = await _erpClient.GetProductAsync(product.Id);
            if (existingProduct == null)
            {
                return await _erpClient.AddProductAsync(product);
            }
            else
            {          
                return await UpdateProductAsync(product, existingProduct);
            }
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var existingProduct = await _erpClient.GetProductAsync(product.Id);
            return await UpdateProductAsync(product, existingProduct);
        }

        private async Task<Product> UpdateProductAsync(Product product, Product existingProduct)
        {
            if (existingProduct == null)
            {
                throw new BidOneException(ExceptionType.NotFound, StringConstants.Validation_ProductNotFound);
            }
            else
            {
                var dupProduct = await _erpClient.GetProductAsync(product.Name);
                if (dupProduct ==null || dupProduct.Id == product.Id)
                {
                    return await _erpClient.UpdateProductAsync(product);
                }
                else
                {
                    throw new BidOneException(ExceptionType.Conflict, StringConstants.Validation_ProductExists);
                }
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _cacheService.GetOrSetAsync(CacheKeys.GetAllProducts, async () =>
            {
                var productsTask = _erpClient.GetProductsAsync();
                var stocksTask = _warehouseClient.GetStocksAsync();

                await Task.WhenAll(productsTask, stocksTask);

                var products = productsTask.Result;
                var stocks = stocksTask.Result;

                foreach (var product in products)
                {
                    var stock = stocks.Where(s => s.ProductId == product.Id);
                    product.StockDetails = stock;
                }

                return products;
            }, TimeSpan.FromSeconds(60)); // TTL 60 seconds
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var product = await _erpClient.GetProductAsync(id);

            if (product == null)
            {
                throw new BidOneException(ExceptionType.NotFound, StringConstants.Validation_ProductNotFound);
            }
            else
            {
                var stock = await _warehouseClient.GetStockAsync(product.Id);
                product.StockDetails = stock;
            }

            return product;
        }

        public async Task<int> DeleteProductAsync(int id)
        {
            var existingProduct = await _erpClient.GetProductAsync(id);
            if (existingProduct == null)
            {
                throw new BidOneException(ExceptionType.NotFound, StringConstants.Validation_ProductNotFound);
            }
            else
            {
                return await _erpClient.DeleteProductAsync(id);
            }
        }
    }
}
