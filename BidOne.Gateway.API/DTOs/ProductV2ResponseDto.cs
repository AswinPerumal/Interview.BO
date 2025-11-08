namespace BidOne.Gateway.API.DTOs
{
    public class ProductV2ResponseDto : ProductResponseDto
    {
        public IEnumerable<StockResponseDto> StockDetails { get; init; }
    }
}
