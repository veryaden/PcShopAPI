using PcShop.Areas.Cart.Dtos;
using PcShop.Areas.Cart.Repositories;
using PcShop.Models;
using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Cart.Model;

namespace PcShop.Areas.Cart.Services
{
    public class CartService : ICartService
    {
        private readonly ExamContext _context;

        public CartService(ExamContext context)
        {
            _context = context;
        }

        public List<CartDto> GetCart(int userId)
        {
            var cartItems = _context.Carts
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.CartItems)
                .Select(ci => new CartDto
                {
                    id = ci.CartItemId,
                    name = ci.Sku.Product.ProductName,
                    spec = ci.Sku.Skuname,
                    price = (int)(ci.Sku.Product.BasePrice + ci.Sku.PriceAdjustment),
                    quantity = ci.Quantity,
                    imageUrl = ci.Sku.Product.ProductImages
                        .Where(pi => pi.IsMainOrNot == 1)
                        .Select(pi => pi.ImageUrl)
                        .FirstOrDefault() ?? "default.jpg",
                    selected = true // Default to true or however you want to handle this
                })
                .ToList();

            return cartItems;
        }

        public bool UpdateCart(int userId, CartItemModel model)
        {
            var query = _context.CartItems
                .Where(o => o.Cart.UserId == userId && o.Skuid == model.CartItemId)
                .FirstOrDefault();

            if (query == null)
            {
                return false;
            }

            query.Quantity = model.Quantity;
            _context.SaveChanges();

            return true;
        }
    }
}
