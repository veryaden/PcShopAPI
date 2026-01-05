namespace PcShop.Areas.Checkout.Dtos
{
    public class CreateOrderDto
    {
        public int ShippingMethodId { get; set; }
        public string ShippingAddress { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }

        public string SelectedGateway { get; set; }
        public string SelectedPayment { get; set; }

        public int? UserCouponId { get; set; }
        public int UsePoints { get; set; }
    }
}
