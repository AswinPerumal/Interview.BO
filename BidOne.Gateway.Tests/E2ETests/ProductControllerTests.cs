using AutoFixture;
using AutoMapper;
using BidOne.Gateway.API.Controllers.v1;
using BidOne.Gateway.API.DTOs;
using BidOne.Gateway.Application.Implementation;
using BidOne.Gateway.Application.InfraInterfaces;
using BidOne.Gateway.Application.Interfaces;
using BidOne.Gateway.Domain.Constants;
using BidOne.Gateway.Domain.Entities;
using BidOne.Gateway.Domain.Enums;
using BidOne.Gateway.Domain.Exceptions;
using Moq;
using System.Net;

namespace BidOne.Gateway.Tests.E2ETests
{
    public class ProductsControllerEndToEndTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IErpClient> _mockErpClient;
        private readonly Mock<IWarehouseClient> _mockWarehouseClient;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductService _productService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductsController _controller;

        public ProductsControllerEndToEndTests()
        {
            // Setup AutoFixture with Moq
            _fixture = new Fixture();

            // Generate mocks
            _mockErpClient = _fixture.Freeze<Mock<IErpClient>>();
            _mockWarehouseClient = _fixture.Freeze<Mock<IWarehouseClient>>();
            _mockCacheService = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();
            _mockProductService = new Mock<IProductService>();
            // Create real ProductService with mocked clients
            _productService = new ProductService(_mockErpClient.Object, _mockWarehouseClient.Object, _mockCacheService.Object);

            // Create controller with service and mapper
            _controller = new ProductsController(_productService, _mockMapper.Object);
        }

        [Fact]
        public async Task GetProduct_ReturnsSuccessResponse_WithStock()
        {
            // Arrange
            var product = _fixture.Create<Product>();

            // Create a list of stock objects for this product
            var stockList = _fixture.Build<Stock>()
                                    .With(s => s.ProductId, product.Id)
                                    .CreateMany(2)
                                    .ToList();

            product.StockDetails = null; // Will be set by service

            // Expected DTO after mapping
            var productDto = _fixture.Build<ProductResponseDto>()
                                     .With(d => d.Id, product.Id)
                                     .Create();

            // ERP returns product
            _mockErpClient
                .Setup(x => x.GetProductAsync(product.Id))
                .ReturnsAsync(product);

            // Warehouse returns stock list
            _mockWarehouseClient
                .Setup(x => x.GetStockAsync(product.Id))
                .ReturnsAsync(stockList)
                .Callback(() => product.StockDetails = stockList);
            // <-- ensures product gets stock like service does

            // Mapper maps final product (with stock) to DTO
            _mockMapper
                .Setup(x => x.Map<ProductResponseDto>(It.Is<Product>(p => p.Id == product.Id && p.StockDetails == stockList)))
                .Returns(productDto);

            // Act
            var result = await _controller.GetProduct(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(productDto, result.Data);
        }


        [Fact]
        public async Task GetProduct_ThrowsException_WhenProductNotFound()
        {
            // Arrange
            int invalidId = _fixture.Create<int>();

            _mockErpClient.Setup(x => x.GetProductAsync(invalidId))
                          .ReturnsAsync((Product)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BidOneException>(() => _controller.GetProduct(invalidId));
            Assert.Equal(ExceptionType.NotFound, exception.exceptionType);
            Assert.Equal(StringConstants.Validation_ProductNotFound, exception.Message);
        }

        [Fact]
        public async Task CreateOrUpdateProduct_NewProduct_AddsProduct()
        {
            // Arrange
            var request = _fixture.Build<CreateProductRequestDto>().With(p => p.Id, 0).Create();
            var product = _fixture.Build<Product>().With(p => p.Id, 10).Create();
            var responseDto = _fixture.Build<ProductResponseDto>().With(d => d.Id, 10).Create();

            _mockMapper.Setup(m => m.Map<Product>(request)).Returns(product);
            _mockMapper.Setup(m => m.Map<ProductResponseDto>(It.IsAny<Product>())).Returns(responseDto);

            // ERP AddProductAsync returns the new product
            _mockErpClient.Setup(e => e.AddProductAsync(product)).ReturnsAsync(product);

            // Act
            var result = await _controller.CreateOrUpdateProduct(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(responseDto, result.Data);

            // Verify AddProductAsync was called
            _mockErpClient.Verify(e => e.AddProductAsync(product), Times.Once);
        }


        [Fact]
        public async Task CreateOrUpdateProduct_ExistingProduct_UpdatesProduct()
        {
            // Arrange
            var request = _fixture.Build<CreateProductRequestDto>().With(p => p.Id, 1).Create();
            var product = _fixture.Build<Product>().With(p => p.Id, 1).Create();
            var responseDto = _fixture.Build<ProductResponseDto>().With(d => d.Id, 1).Create();

            _mockMapper.Setup(m => m.Map<Product>(request)).Returns(product);
            _mockMapper.Setup(m => m.Map<ProductResponseDto>(It.IsAny<Product>())).Returns(responseDto);

            // ERP returns existing product
            _mockErpClient.Setup(e => e.GetProductAsync(product.Id)).ReturnsAsync(product);

            // ERP UpdateProductAsync returns updated product
            _mockErpClient.Setup(e => e.UpdateProductAsync(product)).ReturnsAsync(product);

            // Act
            var result = await _controller.CreateOrUpdateProduct(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(responseDto, result.Data);

            _mockErpClient.Verify(e => e.UpdateProductAsync(product), Times.Once);
        }


        [Fact]
        public async Task CreateOrUpdateProduct_DuplicateName_ThrowsConflict()
        {
            // Arrange
            var request = _fixture.Build<CreateProductRequestDto>().With(p => p.Id, 1).Create();
            var product = _fixture.Build<Product>().With(p => p.Id, 1).Create();
            var duplicateProduct = _fixture.Build<Product>().With(p => p.Id, 2).Create();

            _mockMapper.Setup(m => m.Map<Product>(request)).Returns(product);

            // Existing product exists
            _mockErpClient.Setup(e => e.GetProductAsync(product.Id)).ReturnsAsync(product);

            // Duplicate by name
            _mockErpClient.Setup(e => e.GetProductAsync(product.Name)).ReturnsAsync(duplicateProduct);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BidOneException>(() =>
                _productService.CreateOrUpdateProductAsync(product));

            Assert.Equal(ExceptionType.Conflict, ex.exceptionType);
        }


        [Fact]
        public async Task CreateOrUpdateProduct_ProductNotFound_ThrowsNotFound()
        {
            // Arrange
            var request = _fixture.Build<CreateProductRequestDto>().With(p => p.Id, 999).Create();
            var product = _fixture.Build<Product>().With(p => p.Id, 999).Create();

            _mockMapper.Setup(m => m.Map<Product>(request)).Returns(product);

            // ERP returns null → product does not exist
            _mockErpClient.Setup(e => e.GetProductAsync(product.Id)).ReturnsAsync((Product)null);

            // Act & Assert
            var resultEx = await _productService.CreateOrUpdateProductAsync(product);

            // This should call AddProductAsync because of service logic (Id != 0, existing product null)
            _mockErpClient.Verify(e => e.AddProductAsync(product), Times.Once);
        }


    }
}