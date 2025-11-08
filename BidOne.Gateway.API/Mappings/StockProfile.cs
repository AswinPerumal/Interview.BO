using AutoMapper;
using BidOne.Gateway.API.DTOs;
using BidOne.Gateway.Domain.Entities;

namespace BidOne.Gateway.API.Mappings
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<Stock, StockResponseDto>();            
        }
    }
}
