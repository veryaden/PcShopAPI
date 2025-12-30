using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Models;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IAdminData
    {
        Task<List<Order>> GetOrdersAsync(OrderStatus? status , string? orderno);

        Task<Order?> GetOrderDetailAsync(int orderId, int userId);
    }
}