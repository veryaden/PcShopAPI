using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Models;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IOrderData
    {
        Task<OrderPagedResult<Order>> GetOrdersAsync(int userId, OrderStatus? status, string? orderno, int page, int pageSize);

        Task<Order?> GetOrderDetailAsync(int orderId, int userId);
    }
}