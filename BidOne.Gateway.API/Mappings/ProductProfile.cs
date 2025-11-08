using AutoMapper;
using BidOne.Gateway.API.DTOs;
using BidOne.Gateway.Domain.Entities;

namespace BidOne.Gateway.API.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductRequestDto, Product>();
            CreateMap<UpdateProductRequestDto, Product>();

            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.StockCount, opt => opt.MapFrom(src => src.StockDetails != null
            ? src.StockDetails
                .GroupBy(x => x.ProductId)
                .Select(g => g.Sum(sd => sd.Count))
                .FirstOrDefault()
            : null));

            CreateMap<Product, ProductV2ResponseDto>()
              .ForMember(dest => dest.StockDetails, opt => opt.MapFrom(src => src.StockDetails.Where(x=>x.ProductId == src.Id)));
        }
    }
}
