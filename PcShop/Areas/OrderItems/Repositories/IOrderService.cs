using PcShop.Models;

namespace PcShop.Areas.OrderItems.Repositories
{
    public interface IOrderService
    {
        Task<Order?> GetOrderDetailAsync(int orderId, int userId);
    }
}
