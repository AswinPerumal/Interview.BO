using Asp.Versioning;
using AutoMapper;
using BidOne.Gateway.API.Attributes;
using BidOne.Gateway.API.DTOs;
using BidOne.Gateway.API.Helpers;
using BidOne.Gateway.Application.Interfaces;
using BidOne.Gateway.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BidOne.Gateway.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<APIResponse> GetProducts()
        {
            var products = await _productService.GetAllAsync();
            var productsResponse = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
            return APIResponse.Success(productsResponse);
        }


        [HttpGet("{id}")]
        public async Task<APIResponse> GetProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);
            var productResponse = _mapper.Map<ProductResponseDto>(product);
            return APIResponse.Success(productResponse);
        }

        [HttpPost]
        [Idempotent]
        public async Task<APIResponse> CreateOrUpdateProduct(CreateProductRequestDto productRequest)
        {
            var product = _mapper.Map<Product>(productRequest);
            var p = await _productService.CreateOrUpdateProductAsync(product);
            var productResponse = _mapper.Map<ProductResponseDto>(p);          
            return APIResponse.Success(productResponse);
        }

        [HttpPut("{id}")]
        [Idempotent]
        public async Task<APIResponse> UpdateProduct(int id, UpdateProductRequestDto productRequest)
        {
            var product = _mapper.Map<Product>(productRequest);
            product.Id = id;
            var p = await _productService.UpdateProductAsync(product);
            var productResponse = _mapper.Map<ProductResponseDto>(p);
            return APIResponse.Success(productResponse);
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteProduct(int id)
        {       
            var response = await _productService.DeleteProductAsync(id);
            return APIResponse.Success(response);
        }

    }
}
