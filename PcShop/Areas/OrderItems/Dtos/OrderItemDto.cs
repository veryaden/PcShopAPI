using System;

namespace PcShop.Areas.OrderItems.Dtos
{
    public class OrderItemDto
    {
        public string ProductName { get; set; } = null!;

        public string ProductImage { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPriceAtPurchase { get; set; }
        public decimal SubTotal => Quantity * UnitPriceAtPurchase;
    }
}
