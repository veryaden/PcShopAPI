using PcShop.Areas.Cart.Dtos;
using PcShop.Areas.Cart.Model;

namespace PcShop.Areas.Cart.Repositories
{
    public interface ICartService
    {
        List<CartDto> GetCart(int userId);
        bool UpdateCart(int userId, CartItemModel model);

        bool DeleteCartItem(int userId, int cartItemId);
    }
}
