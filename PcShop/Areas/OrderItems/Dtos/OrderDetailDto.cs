using System;
using System.Collections.Generic;

namespace PcShop.Areas.OrderItems.Dtos
{
    public class OrderDetailsDTO
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscointAmount { get; set; }
        public int UsedPoints { get; set; }
        public string ReceiverName { get; set; }
        public string ShippingMethodName { get; set; }
        public string ShippingAddress { get; set; }
        public string ReceiverPhone { get; set; }
        public string SelectedPayment { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
