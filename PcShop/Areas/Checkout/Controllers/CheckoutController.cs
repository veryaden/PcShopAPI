using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Cart.Repositories;
using PcShop.Areas.Cart.Services;
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);
            var user = _checkoutService.GetUser(userId);
            return Ok(user);
        }
    }

}
