using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Models;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IAdminData
    {
        Task<OrderPagedResult<Order>> GetOrdersAsync(OrderStatus? status, string? orderno, int page, int pageSize);

        Task<Order?> GetOrderDetailAsync(int orderId);
    }
}