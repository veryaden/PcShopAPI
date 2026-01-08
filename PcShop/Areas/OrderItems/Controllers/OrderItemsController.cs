using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.OrderItems.Repositories;
using PcShop.Areas.OrderItems.Dtos;
using System.Security.Claims;

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
            var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) 
                             ?? User.FindFirstValue("sub");
            return int.Parse(userIdClaim);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItems(int orderId)
        {
            int userId = GetUserId();
            var items = await _orderItemsService.GetOrderItemsAsync(orderId, userId);
            return Ok(items);
        }

        [HttpGet("detail/{orderId}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(int orderId)
        {
            int userId = GetUserId();
            var detail = await _orderItemsService.GetOrderDetailAsync(orderId, userId);
            if (detail == null) return NotFound();
            return Ok(detail);
        }
    }
}
