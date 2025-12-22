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
    }
}
