namespace PcShop.Areas.Checkout.Dtos
{
    public class CreateOrderDto
    {
        public int shippingMethodId { get; set; }
        public string shippingAddress { get; set; }
        public string receiverName { get; set; }
        public string receiverPhone { get; set; }

        //public string SelectedGateway { get; set; }
        //public string SelectedPayment { get; set; }
        public string shippingMethod { get; set; }
        public string paymentMethod { get; set; }

        public int? userCouponId { get; set; }
        public int usePoints { get; set; }
    }
}
