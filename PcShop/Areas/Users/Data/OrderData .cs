using Microsoft.EntityFrameworkCore;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.DTO;
using PcShop.Areas.Users.Interface;
using PcShop.Models;

namespace PcShop.Areas.Users.Data
{
    public class OrderData : IOrderData
    {
        private readonly ExamContext _context;

        public OrderData(ExamContext context)
        {
            _context = context;
        }
       
        public async Task<OrderPagedResult<Order>> GetOrdersAsync(int userId, OrderStatus? status, string? orderno, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 4 : pageSize;
            pageSize = Math.Min(pageSize, 50); // 防呆：避免一次拉爆

            var query = _context.Orders.AsNoTracking().Where(o => o.UserId == userId); ;

            if (status.HasValue)
                query = query.Where(o => o.OrderStatus == (int)status.Value);

            if (!string.IsNullOrWhiteSpace(orderno))
                query = query.Where(o => o.OrderNo.Contains(orderno));

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(o => o.CreateDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new OrderPagedResult<Order>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items
            };

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

