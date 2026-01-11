using PcShop.Areas.Cart.Dtos;
using PcShop.Areas.Cart.Model;

namespace PcShop.Areas.Cart.Repositories
{
    public interface ICartService
    {
        List<CartDto> GetCart(int userId);
        bool UpdateCart(int userId, CartItemModel model);
        bool DeleteCartItem(int userId, int cartItemId);

        List<UserCouponDto> GetCoupons(int userId);
        CouponValidationDto ValidateCoupon(int userId, int userCouponId);
        UserCouponDto GetCouponsData(string couponsCode, int userId);  //有新增int userId
        
        // 新增點數功能
        UserPointDto GetUserPoints(int userId);
        PointValidationDto ValidatePoints(int userId, int pointsToUse, int? userCouponId = null);
    }
}
