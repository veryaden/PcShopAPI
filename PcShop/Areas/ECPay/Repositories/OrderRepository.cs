using Microsoft.EntityFrameworkCore;
using PcShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcShop.Areas.ECPay.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, int status);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly ExamContext _context;
        public OrderRepository(ExamContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Sku)
                    .ThenInclude(s => s.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, int status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                order.UpdateDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
