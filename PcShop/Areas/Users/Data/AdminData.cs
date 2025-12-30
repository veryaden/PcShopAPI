using Microsoft.EntityFrameworkCore;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Interface;
using PcShop.Models;

namespace PcShop.Areas.Users.Data
{
    public class AdminData : IAdminData
    {
        private readonly ExamContext _context;

        public AdminData(ExamContext context)
        {
            _context = context;
        }

        //抓Order總表
        public Task<List<Order>> GetOrdersAsync(OrderStatus? status , string? orderno)
        {
            var query = _context.Orders
                .AsNoTracking();
                

            if (status.HasValue)
                query = query.Where(o => o.OrderStatus == (int)status.Value);

            if (!string.IsNullOrWhiteSpace(orderno))
                query = query.Where(o => o.OrderNo.Contains(orderno));
            return query
                .OrderByDescending(o => o.CreateDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderDetailAsync(int orderId, int userId)
        {
            return await _context.Orders
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Sku)
                .ThenInclude(s => s.Product)
                    .ThenInclude(p => p.ProductImages)
        .FirstOrDefaultAsync(o =>
            o.OrderId == orderId &&
            o.UserId == userId);
        }

    }
}

