using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Models;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IOrderService
    {
        Task<OrderPagedResult<OrderListDTO>> GetOrderListAsync(int userId, OrderStatus? status, string? orderno, int page, int pageSize);

        Task<OrderDetailsDTO> GetOrderDetailAsync(int orderId, int userId);
    }
}