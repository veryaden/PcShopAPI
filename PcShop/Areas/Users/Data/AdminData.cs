using Microsoft.EntityFrameworkCore;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.DTO;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.Linq.Dynamic.Core;

namespace PcShop.Areas.Users.Data
{
    public class AdminData : IAdminData
    {
        private readonly ExamContext _context;

        public AdminData(ExamContext context)
        {
            _context = context;
        }

        //抓Order總表 包含filter跟分頁功能
        public async Task<OrderPagedResult<Order>> GetOrdersAsync(OrderStatus? status , string? orderno, int page , int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 4 : pageSize;
            pageSize = Math.Min(pageSize, 50); // 防呆：避免一次拉爆

            var query = _context.Orders.AsNoTracking();
                
            if (status.HasValue)
                query = query.Where(o => o.OrderStatus == (int)status.Value);

            if (!string.IsNullOrWhiteSpace(orderno))
                query = query.Where(o => o.OrderNo.Contains(orderno));

            var total = query.Count();
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

        public async Task<Order?> GetOrderDetailAsync(int orderId)
        {
            return await _context.Orders
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Sku)
                .ThenInclude(s => s.Product)
                    .ThenInclude(p => p.ProductImages)
        .FirstOrDefaultAsync(o =>
            o.OrderId == orderId);
        }

    }
}

