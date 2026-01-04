using System.Collections.Generic;

namespace PcShop.Areas.Checkout.Dtos
{
    public class CheckoutDto
    {
        public List<CheckoutItemDto> Items { get; set; } = new List<CheckoutItemDto>();
        public decimal Subtotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string AppliedCouponCode { get; set; }
        public decimal? CouponDiscountValue { get; set; }
        public string CouponDiscountType { get; set; }
    }

    public class CheckoutItemDto
    {
        public int SkuId { get; set; }
        public string ProductName { get; set; }
        public string SkuName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public decimal ItemSubtotal => Price * Quantity;
    }
}
