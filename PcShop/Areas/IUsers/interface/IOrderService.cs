using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Models;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IOrderService
    {
        Task<List<OrderListDTO>> GetOrderListAsync(int userId,OrderStatus? status);

        Task<OrderDetailsDTO> GetOrderDetailAsync(int orderId, int userId);
    }
}