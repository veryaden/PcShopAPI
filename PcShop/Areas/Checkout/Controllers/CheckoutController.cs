using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Cart.Repositories;
using PcShop.Areas.Cart.Services;
using PcShop.Areas.Checkout.Dtos;
using PcShop.Areas.Checkout.Repositories;
using PcShop.Areas.Checkout.Services;
using System.Security.Claims;

namespace PcShop.Areas.Checkout.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpGet]
        [Authorize]
        [Route("Users")]
        public IActionResult GetUser()
        {
            //取得User資訊用法//
            var userIdClaim = User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);
            var user = _checkoutService.GetUser(userId);
            return Ok(user);
        }
        [HttpPost]
        [Authorize]
        [Route("Create")]
        public IActionResult CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userIdClaim = User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);
            try
            {
                int orderId = _checkoutService.CreateOrder(userId, dto);

                if (orderId <= 0)
                {
                    return BadRequest(new { success = false, message = "建立訂單失敗" });
                }

                return Ok(new { success = true, orderId });
                //int orderId = _checkoutService.CreateOrder(userId, dto);
                //return Ok(new { success = true, orderId = orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }

            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex); // ⭐ 一定要加
            //    return BadRequest(new { success = false, message = ex.Message });
            //}
        }
    }
}
