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

        /// <summary>
        /// 刪除購物車項目
        /// </summary>
        /// <param name="userId">使用者的 ID (來自 Token)</param>
        /// <param name="itemId">要刪除的商品 ID (對應資料庫的 Skuid)</param>
        /// <returns>刪除成功回傳 true，失敗回傳 false</returns>
        public bool DeleteCartItem(int userId, int cartItemId)
        {
            // 1. 查詢資料：確保是「該使用者的購物車」且「包含該商品」
            // 邏輯與你的 UpdateCart 保持一致
            var query = _context.CartItems
                .Where(o => o.Cart.UserId == userId && o.CartItemId == cartItemId)
                .FirstOrDefault();

            // 2. 如果找不到資料 (可能已經被刪除了，或是該使用者根本沒這個商品)
            if (query == null)
            {
                return false;
            }

            // 3. 告訴 EF Core 標記這個物件為「刪除」狀態
            _context.CartItems.Remove(query);

            // 4. 存檔 (這時候才會真的對資料庫下 DELETE SQL 指令)
            _context.SaveChanges();

            return true;
        }
    }
}
