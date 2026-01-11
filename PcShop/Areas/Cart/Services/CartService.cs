using PcShop.Areas.Cart.Dtos;
using PcShop.Areas.Cart.Repositories;
using PcShop.Models;
using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Cart.Model;
using System.Linq.Dynamic.Core;

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
                    price = (int)Math.Round(ci.Sku.Product.BasePrice + ci.Sku.PriceAdjustment, MidpointRounding.AwayFromZero),
                    quantity = ci.Quantity,
                    imageUrl = ci.Sku.Product.ProductImages
                        //.Where(pi => pi.IsMainOrNot !== 1) //抓圖片的.但目前設定是Null所以先註解
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
                .Where(o => o.Cart.UserId == userId && o.CartItemId == model.CartItemId)
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

        // 修改這裡：將 List<UserCoupon> 改為 List<UserCouponDto>
        public List<UserCouponDto> GetCoupons(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            // coupons 的型別是 List<UserCouponDto>
            var coupons = _context.UserCoupons
                .Where(uc => uc.UserId == userId && !uc.IsUsed && uc.Coupon.IsActive && uc.Coupon.ExpirationDate >= today)
                .Select(uc => new UserCouponDto
                {
                    UserCouponId = uc.UserCouponId,
                    CouponCode = uc.Coupon.CouponCode,
                    Name = uc.Coupon.CouponCode, // Using CouponCode as Name for now
                    DiscountType = uc.Coupon.DiscountType,
                    DiscountValue = uc.Coupon.DiscountValue,
                    MinOrderAmount = uc.Coupon.MinOrderAmount,
                    IsActive = uc.Coupon.IsActive
                })
                .ToList();

            return coupons;
        }

        public CouponValidationDto ValidateCoupon(int userId, int userCouponId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var userCoupon = _context.UserCoupons
                .Include(uc => uc.Coupon)
                // 檢查優惠券狀態 (是否領取、是否使用、是否過期)
                .FirstOrDefault(uc => uc.UserCouponId == userCouponId && uc.UserId == userId);

            if (userCoupon == null)
            {
                return new CouponValidationDto { Success = false, Message = "找不到該優惠券" };
            }

            if (userCoupon.IsUsed)
            {
                return new CouponValidationDto { Success = false, Message = "此優惠券已使用過" };
            }

            if (!userCoupon.Coupon.IsActive || userCoupon.Coupon.ExpirationDate < today)
            {
                return new CouponValidationDto { Success = false, Message = "此優惠券已過期或失效" };
            }

            // 計算購物車總金額
            decimal cartTotal = GetCartTotal(userId);

            // 檢查優惠券狀態 (是否領取、是否使用、是否過期) (MinOrderAmount)
            if (cartTotal < userCoupon.Coupon.MinOrderAmount)
            {
                return new CouponValidationDto
                {
                    Success = false,
                    Message = $"未滿最低消費金額 {userCoupon.Coupon.MinOrderAmount:N0} 元",
                    DiscountAmount = 0,
                    FinalTotal = cartTotal
                };
            }

            // 計算折扣金額
            decimal discountAmount = 0;
            if (userCoupon.Coupon.DiscountType.Equals("percentage", StringComparison.OrdinalIgnoreCase))
            {
                // DiscountValue 是折抵百分比 (例如 10 代表折 10%，即打 9 折)
                discountAmount = cartTotal * (userCoupon.Coupon.DiscountValue / 100);
            }
            else
            {
                // 固定金額折抵 (假設除了 percentage 以外都是固定金額，或可再細分)
                discountAmount = userCoupon.Coupon.DiscountValue;
            }

            // 確保折扣不會大於總金額
            if (discountAmount > cartTotal)
            {
                discountAmount = cartTotal;
            }

            return new CouponValidationDto
            {
                Success = true,
                Message = "優惠券套用成功",
                DiscountAmount = Math.Round(discountAmount, 0, MidpointRounding.AwayFromZero),
                FinalTotal = Math.Round(cartTotal - discountAmount, 0, MidpointRounding.AwayFromZero)
            };
        }
        public UserCouponDto GetCouponsData(string couponsCode, int userId)  //有人手把手教學指出來
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            //var query = _context.Coupons.Where(c => c.CouponCode == couponsCode && 
            //        c.IsActive && c.ExpirationDate >= today && c.MaxUses > 0 )
            //    .Select(c => new UserCouponDto()
            //    {                  
            //        CouponCode = c.CouponCode,
            //        Name = c.CouponCode, // Using CouponCode as Name for now
            //        DiscountType = c.DiscountType,
            //        DiscountValue = c.DiscountValue,
            //        MinOrderAmount = c.MinOrderAmount,
            //        IsActive = c.IsActive
            //    }).FirstOrDefault();

            var coupons = _context.UserCoupons
                .Where(uc => uc.UserId == userId && !uc.IsUsed && uc.Coupon.IsActive && uc.Coupon.ExpirationDate >= today && uc.Coupon.CouponCode == couponsCode)
                .Select(uc => new UserCouponDto
                {
                    UserCouponId = uc.UserCouponId,
                    CouponCode = uc.Coupon.CouponCode,
                    Name = uc.Coupon.CouponCode, // Using CouponCode as Name for now
                    DiscountType = uc.Coupon.DiscountType,
                    DiscountValue = uc.Coupon.DiscountValue,
                    MinOrderAmount = uc.Coupon.MinOrderAmount,
                    IsActive = uc.Coupon.IsActive
                })
                .FirstOrDefault();

            return coupons;
        }

        public UserPointDto GetUserPoints(int userId)
        {
            var now = DateTime.Now;
            var thirtyDaysFromNow = now.AddDays(30); //30天內即將到期點數

            var query = _context.GamePoints
                .Where(p => p.UserId == userId 
                            && p.Status == 1 
                            && p.UsedAt == null 
                            && (p.ExpiredAt == null || p.ExpiredAt > now));

            int totalPoints = query.Sum(p => (int?)p.Points) ?? 0;

            int soonExpiring = query
                .Where(p => p.ExpiredAt != null && p.ExpiredAt <= thirtyDaysFromNow)
                .Sum(p => (int?)p.Points) ?? 0;

            return new UserPointDto
            {
                TotalAvailablePoints = totalPoints,
                SoonExpiringPoints = soonExpiring
            };
        }

        public PointValidationDto ValidatePoints(int userId, int pointsToUse, int? userCouponId = null)
        {
            if (pointsToUse < 0)
            {
                return new PointValidationDto { Success = false, Message = "點數不能為負數" };
            }

            // 1. 取得使用者總可用點數
            var userPoints = GetUserPoints(userId);
            int availablePoints = userPoints.TotalAvailablePoints;

            if (pointsToUse > availablePoints)
            {
                return new PointValidationDto 
                { 
                    Success = false, 
                    Message = $"點數不足，目前可用點數為 {availablePoints} 點",
                    PointDiscount = 0,
                    TotalAvailablePoints = availablePoints
                };
            }

            // 2. 計算基礎總金額
            decimal cartTotal = GetCartTotal(userId);
            decimal discountedTotal = cartTotal;
            decimal couponDiscount = 0;

            // 3. 如果有套用優惠券，先扣除優惠券金額
            if (userCouponId.HasValue)
            {
                var couponResult = ValidateCoupon(userId, userCouponId.Value);
                if (couponResult.Success)
                {
                    couponDiscount = couponResult.DiscountAmount;
                    discountedTotal = cartTotal - couponDiscount;
                }
                else
                {
                    return new PointValidationDto
                    {
                        Success = false,
                        Message = "優惠券無效: " + couponResult.Message,
                        OriginalTotal = cartTotal,
                        CouponDiscount = 0,
                        PointDiscount = 0,
                        FinalTotal = cartTotal,
                        TotalAvailablePoints = availablePoints
                    };
                }
            }

            // 4. 計算點數折抵金額 (1點 = 1元)，上限為折扣後的剩餘總額
            int pointDiscount = pointsToUse;

            if (pointDiscount > discountedTotal)
            {
                pointDiscount = (int)Math.Floor(discountedTotal);
            }

            return new PointValidationDto
            {
                Success = true,
                Message = userCouponId.HasValue ? "已扣除優惠券，點數計算成功" : "點數計算成功",
                OriginalTotal = cartTotal,
                CouponDiscount = couponDiscount,
                PointDiscount = pointDiscount,
                FinalTotal = cartTotal - couponDiscount - pointDiscount,
                TotalAvailablePoints = availablePoints
            };
        }

        private decimal GetCartTotal(int userId)
        {
            var cartItems = _context.Carts
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.CartItems)
                .Select(ci => new
                {
                    Price = ci.Sku.Product.BasePrice + ci.Sku.PriceAdjustment,
                    ci.Quantity
                })
                .ToList();

            return cartItems.Sum(ci => Math.Round(ci.Price, 0, MidpointRounding.AwayFromZero) * ci.Quantity);
        }
    }
}
