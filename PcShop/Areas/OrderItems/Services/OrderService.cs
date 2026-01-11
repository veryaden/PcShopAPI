

using Microsoft.EntityFrameworkCore;
using PcShop.Areas.OrderItems.Repositories;
using PcShop.Models;

namespace PcShop.Areas.OrderItems.Services
{
    public class OrderService : IOrderService
    {
        private readonly ExamContext _context;

        public OrderService(ExamContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderDetailAsync(int orderId, int userId)
        {
            return await _context.Orders
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Sku)
                .ThenInclude(s => s.Product)
                    .ThenInclude(p => p.ProductImages)
                    .Include(o => o.ShippingMethod)
                    .Include(o => o.User)
        .FirstOrDefaultAsync(o =>
            o.OrderId == orderId &&
            o.UserId == userId);
        }
    }
}
