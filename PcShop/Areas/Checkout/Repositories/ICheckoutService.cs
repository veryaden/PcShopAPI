using PcShop.Areas.Checkout.Dtos;

namespace PcShop.Areas.Checkout.Repositories
{
    public interface ICheckoutService
    {
        UserProfilesDto GetUser(int userId);
    }
    
}
