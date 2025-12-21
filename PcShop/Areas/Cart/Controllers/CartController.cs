using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Cart.Repositories;
using PcShop.Areas.Cart.Services;

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
        [Route("api/cart")]
        public IActionResult GetCart()
        {
           var aaa = _cartService.GetCart();
            var qqqq = 1;
            return null;
        }
    }
}
