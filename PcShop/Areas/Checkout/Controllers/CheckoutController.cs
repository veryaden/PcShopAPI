using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Cart.Repositories;
using PcShop.Areas.Cart.Services;
using PcShop.Areas.Checkout.Dtos;
using PcShop.Areas.Checkout.Repositories;
using PcShop.Areas.Checkout.Services;
using PcShop.Areas.ECPay.Services;
using PcShop.Areas.ECPay.Dtos;
using System.Security.Claims;

namespace PcShop.Areas.Checkout.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly PcShop.Areas.Checkout.Services.ICheckoutService _checkoutService;
        private readonly IECPayService _ecpayService;
        public CheckoutController(PcShop.Areas.Checkout.Services.ICheckoutService checkoutService, IECPayService ecpayService)
        {
            _checkoutService = checkoutService;
            _ecpayService = ecpayService;
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
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
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

                // --- 整合金流邏輯 ---
                if (dto.paymentMethod == "ecpay")
                {
                    var ecpayRequest = new ECPayRequestDto 
                    { 
                        OrderId = orderId,
                        TradeDesc = "PcShop Order Payment"
                    };
                    string htmlForm = await _ecpayService.GetECPayParameters(ecpayRequest);
                    return Ok(new { success = true, orderId, htmlForm });
                }

                return Ok(new { success = true, orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
