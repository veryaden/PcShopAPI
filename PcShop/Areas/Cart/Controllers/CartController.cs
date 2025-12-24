using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Cart.Model;
using PcShop.Areas.Cart.Repositories;
using System.Security.Claims;

namespace PcShop.Areas.Cart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetCart()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);
            var cart = _cartService.GetCart(userId);
            return Ok(cart);
        }

        [HttpPost]
        [Authorize]
        [Route("Update")]
        public IActionResult UpdateCart([FromBody] CartItemModel model)
        {
            //取得使用者ID
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //如果沒取得到就回傳401 沒權限的意思
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            //把字串轉成int 因為你的資料庫的userid是int 這樣就可以下條件比對
            int userId = int.Parse(userIdClaim);
            var cart = _cartService.UpdateCart(userId, model);
            return Ok(cart);
        }

        [HttpDelete]
        [Authorize]
        [Route("Delete/{cartItemId}")]
        //要改
        public IActionResult DeleteCart(int cartItemId)  
        {
            //取得使用者ID
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //如果沒取得到就回傳401 沒權限的意思,來防止轉型錯誤，更加安全
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            // 呼叫剛剛寫好的 Service
            bool isDeleted = _cartService.DeleteCartItem(userId, cartItemId);

            if (isDeleted)
            {
                return Ok(new { message = "刪除成功" });
            }
            else
            {
                // 如果回傳 false，可能是 ID 傳錯了找不到該商品
                return NotFound("找不到該商品或刪除失敗");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("Coupons")]
        public IActionResult GetCoupons()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("無效的使用者身分");
            }

            int userId = int.Parse(userIdClaim);
            var coupons = _cartService.GetCoupons(userId);
            return Ok(coupons);
        }

        [HttpPost]
        [Authorize]
        [Route("ValidateCoupon")]
        public IActionResult ValidateCoupon([FromBody] int userCouponId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);
            var result = _cartService.ValidateCoupon(userId, userCouponId);
            return Ok(result);
        }
    }
}
    
