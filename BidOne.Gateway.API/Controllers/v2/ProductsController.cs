using Asp.Versioning;
using AutoMapper;
using BidOne.Gateway.API.DTOs;
using BidOne.Gateway.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BidOne.Gateway.API.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
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
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
        {
            var products = await _productService.GetAllAsync();
            var productsResponse = _mapper.Map<IEnumerable<ProductV2ResponseDto>>(products);
            return Ok(productsResponse);
        }      

    }
}
