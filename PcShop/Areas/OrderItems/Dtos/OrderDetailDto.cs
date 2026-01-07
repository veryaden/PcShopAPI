using System;

namespace PcShop.Areas.OrderItems.Dtos
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public decimal TotalAmount { get; set; }
        public int OrderStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal ShippingFee { get; set; }
        public int UsedPoints { get; set; }
        public decimal DiscountAmount { get; set; }
        public string ShippingMethodName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ShippingAddress { get; set; }
        public string CouponCode { get; set; }
        public int? CouponId { get; set; }
        public decimal CouponDiscount { get; set; }  // 純優惠券的折扣金額（排除了點數折抵）。
        public string CouponDiscountType { get; set; } //折扣類型（例如 percentage 或 fixed）。
        public decimal? CouponDiscountValue { get; set; } //折扣的數值（例如 10% 或 100元）。
        public List<OrderItemDto> Items { get; set; }
    }
}
