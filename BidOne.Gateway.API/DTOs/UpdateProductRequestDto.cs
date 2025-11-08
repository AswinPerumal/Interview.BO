using System.ComponentModel.DataAnnotations;

namespace BidOne.Gateway.API.DTOs
{
    public record UpdateProductRequestDto
    {
        [Required]
        public string Name { get; init; }
        [Required]
        public decimal Price { get; init; }
    }
}
