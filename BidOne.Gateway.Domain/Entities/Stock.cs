namespace BidOne.Gateway.Domain.Entities
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? Count { get; set; }
        public string WarehouseAddress { get; set; }
    }
}
