namespace BidOne.Gateway.API.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public int? StockCount { get; init; }
    }
}
