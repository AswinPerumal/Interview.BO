using System.ComponentModel.DataAnnotations;

namespace BidOne.Gateway.API.DTOs
{
    public record CreateProductRequestDto
    {
        public int Id { get; init; }
        [Required]
        public string Name { get; init; }
        [Required]
        public decimal Price { get; init; }
    }
}
