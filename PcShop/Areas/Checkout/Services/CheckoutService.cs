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
                    ShippingAddress = u.ShippingAddress
                }).FirstOrDefault();

            return query;
        }

        //public int CreateOrder(int userId, CreateOrderDto dto)
        //{
        //    // 確保引用 Microsoft.EntityFrameworkCore 來捕捉併發錯誤
        //    using (var transaction = _context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // 1. 取得購物車資料與驗證
        //            var cart = _context.Carts
        //                .Include(c => c.CartItems)
        //                .ThenInclude(ci => ci.Sku)
        //                .ThenInclude(s => s.Product)
        //                .FirstOrDefault(c => c.UserId == userId);

        //            if (cart == null || !cart.CartItems.Any())
        //            {
        //                throw new Exception("購物車是空的");
        //            }

        //            // 2. 計算商品總額 (Subtotal)
        //            decimal subtotal = cart.CartItems.Sum(ci =>
        //                Math.Round(ci.Sku.Product.BasePrice + ci.Sku.PriceAdjustment, 0, MidpointRounding.AwayFromZero) * ci.Quantity);

        //            // 3. 取得運費與配送方式
        //            string dbLogisticsType = dto.ShippingMethod;
        //            // 處理前端 value 與資料庫 LogisticsType 的對應
        //            if (dto.ShippingMethod == "mainland_delivery")
        //            {
        //                dbLogisticsType = "HOME";
        //            }
        //            // 擴充其他對應 (若有的話)，例如離島
        //            else if (dto.ShippingMethod == "island_delivery")
        //            {
        //                // dbLogisticsType = "ISLAND"; // 依你的資料庫設定調整
        //            }

        //            var shippingMethod = _context.ShippingMethods.FirstOrDefault(s => s.LogisticsType == dbLogisticsType);
        //            if (shippingMethod == null) throw new Exception($"無效的配送方式: {dto.ShippingMethod}");
        //            decimal shippingFee = shippingMethod.Price;

        //            // 4. 計算優惠券折扣
        //            decimal couponDiscount = 0;
        //            if (dto.UserCouponId.HasValue)
        //            {
        //                // 這裡必須檢查 !uc.IsUsed，防止併發時重複使用
        //                var userCoupon = _context.UserCoupons
        //                    .Include(uc => uc.Coupon)
        //                    .FirstOrDefault(uc => uc.UserCouponId == dto.UserCouponId && uc.UserId == userId && !uc.IsUsed);

        //                if (userCoupon != null)
        //                {
        //                    if (subtotal >= userCoupon.Coupon.MinOrderAmount)
        //                    {
        //                        if (userCoupon.Coupon.DiscountType.Equals("percentage", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            couponDiscount = Math.Round(subtotal * (userCoupon.Coupon.DiscountValue / 100), 0, MidpointRounding.AwayFromZero);
        //                        }
        //                        else
        //                        {
        //                            couponDiscount = userCoupon.Coupon.DiscountValue;
        //                        }
        //                        // 標記優惠券已使用
        //                        userCoupon.IsUsed = true;
        //                        userCoupon.UsedDate = DateTime.Now;
        //                    }
        //                }
        //                else
        //                {
        //                    // 若前端傳了 ID 但後端找不到或已使用，視為無效操作或重複請求
        //                    // 這裡可以選擇拋錯，或僅忽略折扣
        //                    // throw new Exception("優惠券無效或已使用"); 
        //                }
        //            }

        //            // 5. 計算點數折抵 (1點 = 1元)
        //            decimal amountAfterCoupon = subtotal - couponDiscount;
        //            int pointsToUse = dto.UsePoints;
        //            if (pointsToUse > amountAfterCoupon) pointsToUse = (int)Math.Floor(amountAfterCoupon);

        //            // 檢查使用者點數是否足夠
        //            var now = DateTime.Now;
        //            var availablePoints = _context.GamePoints
        //                .Where(p => p.UserId == userId && p.Status == 1 && (p.ExpiredAt == null || p.ExpiredAt > now))
        //                .Sum(p => (int?)p.Points) ?? 0;

        //            if (dto.UsePoints > availablePoints) throw new Exception("點數不足");

        //            // 6. 產生單號 (ORD + yyyyMMddHHmmss + random 3碼)
        //            string orderNo = $"ORD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";

        //            // 7. 建立 Order 主表紀錄
        //            var order = new Order
        //            {
        //                UserId = userId,
        //                OrderNo = orderNo,
        //                ShippingMethodId = shippingMethod.ShippingMethodId,
        //                ShippingAddress = dto.ShippingAddress,
        //                ReceiverName = dto.ReceiverName,
        //                ReceiverPhone = dto.ReceiverPhone,
        //                SelectedGateway = dto.ShippingMethod,
        //                SelectedPayment = dto.PaymentMethod,
        //                ShippingFee = shippingFee,
        //                UsedPoints = pointsToUse,
        //                DiscountAmount = couponDiscount + pointsToUse,
        //                TotalAmount = subtotal + shippingFee - couponDiscount - pointsToUse,
        //                OrderStatus = 1, // 1 是待付款
        //                CreateDate = DateTime.Now,
        //                UserCouponId = dto.UserCouponId
        //            };

        //            _context.Orders.Add(order);

        //            // ⭐ 關鍵修正 1：這裡先存檔以取得 OrderId，並捕捉併發錯誤 (例如優惠券同時被搶用)
        //            try
        //            {
        //                _context.SaveChanges();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                // 當發生 "expected to affect 1 row(s) but actually affected 0" 時會進來這裡
        //                throw new Exception("訂單建立失敗：優惠券已被使用或重複操作，請重新整理頁面。");
        //            }

        //            // 8. 建立 OrderItems 明細
        //            foreach (var ci in cart.CartItems)
        //            {
        //                var orderItem = new OrderItem
        //                {
        //                    OrderId = order.OrderId, // 此時已經有 ID 了
        //                    Skuid = ci.Skuid,
        //                    Quantity = ci.Quantity,
        //                    PriceAtPurchase = Math.Round(ci.Sku.Product.BasePrice + ci.Sku.PriceAdjustment, 0, MidpointRounding.AwayFromZero)
        //                };
        //                _context.OrderItems.Add(orderItem);
        //            }

        //            // 9. 扣除點數 Package 邏輯
        //            int remainingPointsToDeduct = pointsToUse;
        //            if (remainingPointsToDeduct > 0)
        //            {
        //                var pointPackets = _context.GamePoints
        //                    .Where(p => p.UserId == userId && p.Status == 1 && (p.ExpiredAt == null || p.ExpiredAt > now))
        //                    .OrderBy(p => p.ExpiredAt ?? DateTime.MaxValue)
        //                    .ToList();

        //                foreach (var packet in pointPackets)
        //                {
        //                    if (remainingPointsToDeduct <= 0) break;

        //                    if (packet.Points <= remainingPointsToDeduct)
        //                    {
        //                        // 整包用完
        //                        remainingPointsToDeduct -= packet.Points;
        //                        packet.Status = 0; // 標記已使用
        //                        packet.UsedAt = DateTime.Now;
        //                        packet.UsedInOrderId = order.OrderId;
        //                    }
        //                    else
        //                    {
        //                        // 部分使用：拆分 Packet
        //                        // 1. 建立一個 已使用的紀錄 關聯到訂單
        //                        var usedPacket = new GamePoint
        //                        {
        //                            UserId = userId,
        //                            Source = packet.Source,
        //                            Points = remainingPointsToDeduct,
        //                            ObtainedAt = packet.ObtainedAt,
        //                            ExpiredAt = packet.ExpiredAt,
        //                            Status = 0, // 已使用狀態
        //                            UsedAt = DateTime.Now,
        //                            UsedInOrderId = order.OrderId
        //                        };
        //                        _context.GamePoints.Add(usedPacket);

        //                        // 2. 更新原 Packet 剩下的點數
        //                        packet.Points -= remainingPointsToDeduct;
        //                        remainingPointsToDeduct = 0;
        //                    }
        //                }
        //            }

        //            // 選擇性：清空購物車 (視你的需求而定，通常結帳完要清空)
        //            //_context.Carts.Remove(cart); // 或只移除 CartItems

        //            // ⭐ 關鍵修正 2：補上最後的 SaveChanges
        //            // 如果沒加這行，上面的 OrderItems 和 GamePoints 扣點動作通通不會存進資料庫！
        //            //_context.SaveChanges();

        //            _context.CartItems.RemoveRange(cart.CartItems); // ✅ 正確：只清空明細

        //            // 10. 儲存變更
        //            _context.SaveChanges();

        //            transaction.Commit();
        //            return order.OrderId;
        //        }
        //        catch (Exception)
        //        {
        //            transaction.Rollback(); // 發生任何錯誤都回滾
        //            throw; // 往外拋出錯誤給 Controller
        //        }
        //    }
        //}
        public int CreateOrder(int userId, CreateOrderDto dto)
        {
            // 確保引用 Microsoft.EntityFrameworkCore 來捕捉併發錯誤
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. 取得購物車資料與驗證
                    var cart = _context.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Sku)
                        .ThenInclude(s => s.Product)
                        .FirstOrDefault(c => c.UserId == userId);

                    if (cart == null || !cart.CartItems.Any())
                    {
                        throw new Exception("購物車是空的");
                    }

                    // 2. 計算商品總額 (Subtotal)
                    decimal subtotal = cart.CartItems.Sum(ci =>
                        Math.Round(ci.Sku.Product.BasePrice + ci.Sku.PriceAdjustment, 0, MidpointRounding.AwayFromZero) * ci.Quantity);

                    // 3. 取得運費與配送方式
                    string dbLogisticsType = dto.ShippingMethod;
                    if (dto.ShippingMethod == "mainland_delivery")
                    {
                        dbLogisticsType = "HOME";
                    }
                    // 擴充其他對應 (若有的話)
                    else if (dto.ShippingMethod == "island_delivery")
                    {
                        // dbLogisticsType = "ISLAND";
                    }

                    var shippingMethod = _context.ShippingMethods.FirstOrDefault(s => s.LogisticsType == dbLogisticsType);
                    if (shippingMethod == null) throw new Exception($"無效的配送方式: {dto.ShippingMethod}");
                    decimal shippingFee = shippingMethod.Price;

                    // 4. 計算優惠券折扣
                    decimal couponDiscount = 0;
                    if (dto.UserCouponId.HasValue)
                    {
                        var userCoupon = _context.UserCoupons
                            .Include(uc => uc.Coupon)
                            .FirstOrDefault(uc => uc.UserCouponId == dto.UserCouponId && uc.UserId == userId && !uc.IsUsed);

                        if (userCoupon != null)
                        {
                            if (subtotal >= userCoupon.Coupon.MinOrderAmount)
                            {
                                if (userCoupon.Coupon.DiscountType.Equals("percentage", StringComparison.OrdinalIgnoreCase))
                                {
                                    couponDiscount = Math.Round(subtotal * (userCoupon.Coupon.DiscountValue / 100), 0, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    couponDiscount = userCoupon.Coupon.DiscountValue;
                                }
                                // 標記優惠券已使用
                                userCoupon.IsUsed = true;
                                userCoupon.UsedDate = DateTime.Now;
                            }
                        }
                    }

                    // 5. 計算點數折抵 (1點 = 1元)
                    decimal amountAfterCoupon = subtotal - couponDiscount;
                    int pointsToUse = dto.UsePoints;
                    if (pointsToUse > amountAfterCoupon) pointsToUse = (int)Math.Floor(amountAfterCoupon);

                    // 檢查使用者點數是否足夠
                    var now = DateTime.Now;
                    var availablePoints = _context.GamePoints
                        .Where(p => p.UserId == userId && p.Status == 1 && (p.ExpiredAt == null || p.ExpiredAt > now))
                        .Sum(p => (int?)p.Points) ?? 0;

                    if (dto.UsePoints > availablePoints) throw new Exception("點數不足");

                    // 6. 產生單號
                    string orderNo = $"ORD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";

                    // 7. 建立 Order 主表紀錄
                    var order = new Order
                    {
                        UserId = userId,
                        OrderNo = orderNo,
                        ShippingMethodId = shippingMethod.ShippingMethodId,
                        ShippingAddress = dto.ShippingAddress,
                        ReceiverName = dto.ReceiverName,
                        ReceiverPhone = dto.ReceiverPhone,
                        SelectedGateway = dto.ShippingMethod,
                        SelectedPayment = dto.PaymentMethod,
                        ShippingFee = shippingFee,
                        UsedPoints = pointsToUse,
                        DiscountAmount = couponDiscount + pointsToUse,
                        TotalAmount = subtotal + shippingFee - couponDiscount - pointsToUse,
                        OrderStatus = 1, // 1 是待付款
                        CreateDate = DateTime.Now,
                        UserCouponId = dto.UserCouponId
                    };

                    _context.Orders.Add(order);

                    // ⭐ 修正1：先儲存訂單與優惠券狀態，並捕捉併發錯誤
                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw new Exception("訂單處理中或優惠券已被搶用，請勿重複刷新頁面。");
                    }

                    // 8. 建立 OrderItems 明細
                    foreach (var ci in cart.CartItems)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = order.OrderId,
                            Skuid = ci.Skuid,
                            Quantity = ci.Quantity,
                            PriceAtPurchase = Math.Round(ci.Sku.Product.BasePrice + ci.Sku.PriceAdjustment, 0, MidpointRounding.AwayFromZero)
                        };
                        _context.OrderItems.Add(orderItem);
                    }

                    // 9. 扣除點數 Package 邏輯
                    int remainingPointsToDeduct = pointsToUse;
                    if (remainingPointsToDeduct > 0)
                    {
                        var pointPackets = _context.GamePoints
                            .Where(p => p.UserId == userId && p.Status == 1 && (p.ExpiredAt == null || p.ExpiredAt > now))
                            .OrderBy(p => p.ExpiredAt ?? DateTime.MaxValue)
                            .ToList();

                        foreach (var packet in pointPackets)
                        {
                            if (remainingPointsToDeduct <= 0) break;

                            if (packet.Points <= remainingPointsToDeduct)
                            {
                                // 整包用完
                                remainingPointsToDeduct -= packet.Points;
                                packet.Status = 0;
                                packet.UsedAt = DateTime.Now;
                                packet.UsedInOrderId = order.OrderId;
                            }
                            else
                            {
                                // 部分使用
                                var usedPacket = new GamePoint
                                {
                                    UserId = userId,
                                    Source = packet.Source,
                                    Points = remainingPointsToDeduct,
                                    ObtainedAt = packet.ObtainedAt,
                                    ExpiredAt = packet.ExpiredAt,
                                    Status = 0,
                                    UsedAt = DateTime.Now,
                                    UsedInOrderId = order.OrderId
                                };
                                _context.GamePoints.Add(usedPacket);

                                packet.Points -= remainingPointsToDeduct;
                                remainingPointsToDeduct = 0;
                            }
                        }
                    }

                    // ⭐ 修正2：儲存明細與點數扣除
                    // 這裡如果失敗(如點數併發)，必須拋出異常回滾
                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw new Exception("點數已被使用或狀態變更，請重新整理頁面。");
                    }

                    // ⭐ 修正3：最後清空購物車 (使用 RemoveRange)
                    // 這裡使用 try-catch 包覆，若購物車已經空了(併發刪除)，則忽略錯誤
                    if (cart.CartItems != null && cart.CartItems.Any())
                    {
                        _context.CartItems.RemoveRange(cart.CartItems);
                        try
                        {
                            _context.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            // 忽略錯誤：購物車已經被清空了，目標達成
                        }
                    }

                    transaction.Commit(); // 提交交易
                    return order.OrderId;
                }
                catch (Exception)
                {
                    transaction.Rollback(); // 發生任何錯誤都回滾
                    throw;
                }
            }
        }
        // 10. 清空購物車
        //_context.CartItems.RemoveRange(cart.CartItems);

        //            _context.SaveChanges();
        //            transaction.Commit();

        //            return order.OrderId;
        //        }
        //        catch (Exception ex) //抓錯誤訊息
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
    }
    }

