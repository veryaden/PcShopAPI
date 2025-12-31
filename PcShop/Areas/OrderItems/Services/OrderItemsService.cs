using PcShop.Areas.Checkout.Repositories;
using PcShop.Areas.OrderItems.Repositories;
using PcShop.Models;

namespace PcShop.Areas.OrderItems.Services
{
    public class OrderItemsService : IOrderItemsService
    {
        private readonly ExamContext _context;

        public OrderItemsService(ExamContext context)
        {
            _context = context;
        }
    }
}
