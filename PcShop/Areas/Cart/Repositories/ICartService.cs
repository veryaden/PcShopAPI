using PcShop.Areas.Cart.Dtos;

namespace PcShop.Areas.Cart.Repositories
{
    public interface ICartService
    {
        CartDto GetCart();
    }
}
