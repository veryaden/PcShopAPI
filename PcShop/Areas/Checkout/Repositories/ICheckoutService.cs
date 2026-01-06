using PcShop.Areas.Checkout.Dtos;

namespace PcShop.Areas.Checkout.Repositories
{
    public interface ICheckoutService
    {
        /// <summary>
        /// 取得使用者基本資料 (用於結帳頁面預填)
        /// </summary>
        UserProfilesDto GetUser(int userId);

        /// <summary>
        /// 建立訂單：包含購物車驗證、優惠券計算、點數扣除、建立訂單明細及清空購物車
        /// </summary>
        /// <returns>回傳產生的 OrderId</returns>
        int CreateOrder(int userId, CreateOrderDto dto);
    }
}


