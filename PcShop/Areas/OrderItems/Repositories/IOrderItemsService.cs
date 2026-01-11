using PcShop.Areas.OrderItems.Dtos;

namespace PcShop.Areas.OrderItems.Repositories
{
    public interface IOrderItemsService
    {
        Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId, int userId);
        Task<OrderDetailDto> GetOrderDetailAsync(int orderId, int userId);

        Task<bool> CancelOrderAsync(int orderId, int userId);
    }
}
