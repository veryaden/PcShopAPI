using System;

namespace PcShop.Areas.Cart.Dtos
{
    public class UserCouponDto
    {
        public int CouponID { get; set; } 

        public string Couponcode { get; set; } //對應前端code

        //public string Name  { get; set; } // 折價券名稱(前端需求)

        public string DiscountType { get; set; } //對應前端type

        public decimal DiscountValue { get; set; } //對應前端value

        public decimal MinOrderAmount { get; set; } //對應前端minSpend 

        public bool IsActive { get; set; }       // 對應 Coupon.IsActive
    }
}
