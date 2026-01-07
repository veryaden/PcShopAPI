using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.OrderItems.Repositories;
using PcShop.Areas.OrderItems.Dtos;

namespace PcShop.Areas.OrderItems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemsService _orderItemsService;
        public OrderItemsController(IOrderItemsService orderItemsService)
        {
            _orderItemsService = orderItemsService;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItems(int orderId)
        {
            var items = await _orderItemsService.GetOrderItemsAsync(orderId);
            return Ok(items);
        }

        [HttpGet("detail/{orderId}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(int orderId)
        {
            var detail = await _orderItemsService.GetOrderDetailAsync(orderId);
            if (detail == null) return NotFound();
            return Ok(detail);
        }
    }
}
