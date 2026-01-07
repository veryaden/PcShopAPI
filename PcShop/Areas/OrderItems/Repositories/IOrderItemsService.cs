using PcShop.Areas.OrderItems.Dtos;

namespace PcShop.Areas.OrderItems.Repositories
{
    public interface IOrderItemsService
    {
        Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId);
        Task<OrderDetailDto> GetOrderDetailAsync(int orderId);
    }
}
