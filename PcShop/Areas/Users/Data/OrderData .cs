using Microsoft.EntityFrameworkCore;
using PcShop.Areas.IUsers.Interface;
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
        public Task<List<Order>> GetOrdersAsync(int userId, OrderStatus? status)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId);

            if (status.HasValue)
                query = query.Where(o => o.OrderStatus == (int)status.Value);

            return query
                .OrderByDescending(o => o.CreateDate)
                .ToListAsync();
        }
    }
}

