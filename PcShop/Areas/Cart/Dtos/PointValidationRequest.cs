namespace PcShop.Areas.Cart.Dtos
{
    public class PointValidationRequest
    {
        public int PointsToUse { get; set; }
        public int? UserCouponId { get; set; }
    }
}
