using PcShop.Areas.Cart.Repositories;
using PcShop.Areas.Checkout.Dtos;
using PcShop.Areas.Checkout.Repositories;
using PcShop.Models;

namespace PcShop.Areas.Checkout.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ExamContext _context;

        public CheckoutService(ExamContext context)
        {
            _context = context;
        }
        public UserProfilesDto GetUser(int userId)
        {
            var query = _context.UserProfiles.Where(u => u.UserId == userId)
                .Select(u => new UserProfilesDto()
                {
                    FullName = u.FullName,
                    Email = u.Mail,
                    Phone = u.Phone,
                    ShippingAddress = u.ShippingAddress
                }).FirstOrDefault();

            return query;
        }
    }
}
