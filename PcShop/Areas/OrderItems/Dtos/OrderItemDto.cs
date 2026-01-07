using System;

namespace PcShop.Areas.OrderItems.Dtos
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int Skuid { get; set; }
        public string ProductName { get; set; }
        public string SkuName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public string ImageUrl { get; set; }
    }
}
