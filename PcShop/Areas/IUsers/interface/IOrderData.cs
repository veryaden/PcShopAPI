using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Models;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IOrderData
    {
        Task<List<Order>> GetOrdersAsync(int userId, OrderStatus? status);
    }
}