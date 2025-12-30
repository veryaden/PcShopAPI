using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Models;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IAdminServices
    {
        Task<List<OrderListDTO>> GetOrderListAsync(OrderStatus? status , string? orderno);

        Task<OrderDetailsDTO> GetOrderDetailAsync(int orderId, int userId);
    }
}