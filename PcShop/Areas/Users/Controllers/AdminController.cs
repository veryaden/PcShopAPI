using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Data;
using System.Security.Claims;

namespace PcShop.Areas.Users.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
       private readonly IAdminServices _admin;
        public AdminController(IAdminServices admin)
        {
            _admin = admin;
        }
        private int GetUserIdOrThrow()
        {
            // 常見：ClaimTypes.NameIdentifier (例如 "1")
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 有些 JWT 會把 user id 放在 "sub"
            if (string.IsNullOrEmpty(idStr))
                idStr = User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(idStr))
                throw new UnauthorizedAccessException("Token 缺少 userId claim (NameIdentifier/sub)");

            if (!int.TryParse(idStr, out var userId))
                throw new UnauthorizedAccessException($"Token userId 不是數字：{idStr}");

            return userId;
        }
        [Authorize]
        [HttpGet("orders")]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderStatus? status, string? orderno)
        {
            var result = await _admin.GetOrderListAsync(status , orderno);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("orders/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            int userId = GetUserIdOrThrow();
            var result = await _admin.GetOrderDetailAsync(orderId, userId);
            return Ok(result);
        }

    }
}
