using PcShop.Models;

namespace PcShop.Areas.Cart.Services
{
    public class UserCouponsService
    {
        private readonly ExamContext _context;

        public UserCouponsService(ExamContext context)
        {
            _context = context;
        }
    }
}
