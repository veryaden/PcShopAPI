using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.OrderItems.Repositories;
using PcShop.Areas.OrderItems.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace PcShop.Areas.OrderItems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemsService _orderItemsService;
        public OrderItemsController(IOrderItemsService orderItemsService)
        {
            _orderItemsService = orderItemsService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return 0;
            return int.Parse(userIdClaim);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItems(int orderId)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var items = await _orderItemsService.GetOrderItemsAsync(orderId, userId);
            return Ok(items);
        }

        [HttpGet("detail/{orderId}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(int orderId)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var detail = await _orderItemsService.GetOrderDetailAsync(orderId, userId);
            if (detail == null) return NotFound();
            return Ok(detail);
        }

        [HttpPost("cancelOrder/{orderId}")]
        public async Task<ActionResult<bool>> CancelOrder(int orderId)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var data = await _orderItemsService.CancelOrderAsync(orderId, userId);
            if (data == null) return NotFound();
            return Ok(data);
        }
    }
}
