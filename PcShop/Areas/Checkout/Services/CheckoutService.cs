using Microsoft.EntityFrameworkCore;
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

        public CheckoutDto GetCheckoutData(int userId, string couponCode = null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // 1. 取得購物車項目與相關產品資訊
            var cartItems = _context.Carts
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.CartItems)
                .Select(ci => new CheckoutItemDto
                {
                    SkuId = ci.Skuid,
                    ProductName = ci.Sku.Product.ProductName,
                    SkuName = ci.Sku.Skuname,
                    Price = Math.Round(ci.Sku.Product.BasePrice + ci.Sku.PriceAdjustment, 0, MidpointRounding.AwayFromZero),
                    Quantity = ci.Quantity,
                    ImageUrl = ci.Sku.Product.ProductImages
                        .OrderByDescending(pi => pi.IsMainOrNot)
                        .Select(pi => pi.ImageUrl)
                        .FirstOrDefault() ?? "default.jpg"
                })
                .ToList();

            if (!cartItems.Any())
            {
                return null;
            }

            var checkoutDto = new CheckoutDto
            {
                Items = cartItems,
                Subtotal = cartItems.Sum(i => i.Price * i.Quantity)
            };

            // 2. 處理優惠券邏輯 (如果有的話)
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = _context.Coupons
                    .FirstOrDefault(c => c.CouponCode == couponCode && c.IsActive && c.ExpirationDate >= today);

                if (coupon != null && checkoutDto.Subtotal >= coupon.MinOrderAmount)
                {
                    checkoutDto.AppliedCouponCode = coupon.CouponCode;
                    checkoutDto.CouponDiscountType = coupon.DiscountType;
                    checkoutDto.CouponDiscountValue = coupon.DiscountValue;

                    decimal discount = 0;
                    if (coupon.DiscountType.Equals("percentage", StringComparison.OrdinalIgnoreCase))
                    {
                        // 例如 DiscountValue 為 10 代表 10%
                        discount = checkoutDto.Subtotal * (coupon.DiscountValue / 100);
                    }
                    else
                    {
                        // 固定金額
                        discount = coupon.DiscountValue;
                    }

                    // 確保折扣不超過小計
                    checkoutDto.DiscountAmount = Math.Min(Math.Round(discount, 0, MidpointRounding.AwayFromZero), checkoutDto.Subtotal);
                }
            }

            // 3. 計算最終總額
            checkoutDto.TotalAmount = checkoutDto.Subtotal - (checkoutDto.DiscountAmount ?? 0);

            return checkoutDto;
        }

        public UserProfilesDto GetUser(int userId)
        {
            var query = _context.UserProfiles.Where(u => u.UserId == userId)
                .Select(u => new UserProfilesDto()
                {
                    FullName = u.FullName,
                    Email = u.Mail,
                    Phone = u.Phone,
                    Address = u.Address
                }).FirstOrDefault();

            return query;
        }
    }
}
