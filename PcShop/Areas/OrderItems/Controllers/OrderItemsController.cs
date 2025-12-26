using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Cart.Repositories;
using PcShop.Areas.OrderItems.Repositories;

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
    }
}
