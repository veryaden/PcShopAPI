using Microsoft.EntityFrameworkCore;
using PcShop.Areas.OrderItems.Dtos;
using PcShop.Areas.OrderItems.Repositories;
using PcShop.Models;

namespace PcShop.Areas.OrderItems.Services
{
    public class OrderItemsService : IOrderItemsService
    {
        private readonly ExamContext _context;

        public OrderItemsService(ExamContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId, int userId) 
        {
            var items = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && oi.Order.UserId == userId)
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    Skuid = oi.Skuid,
                    ProductName = oi.Sku.Product.ProductName,
                    SkuName = oi.Sku.Skuname,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    ImageUrl = oi.Sku.Product.ProductImages
                        .Where(pi => pi.IsMainOrNot == 1)
                        .Select(pi => pi.ImageUrl)
                        .FirstOrDefault() ?? oi.Sku.Product.ProductImages
                        .OrderBy(pi => pi.ImageSortBy)
                        .Select(pi => pi.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return items;
        }

        public async Task<OrderDetailDto> GetOrderDetailAsync(int orderId, int userId)
        {
            var order = await _context.Orders
                .Where(o => o.OrderId == orderId && o.UserId == userId)
                .Select(o => new OrderDetailDto
                {
                    OrderId = o.OrderId,
                    OrderNo = o.OrderNo,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    StatusName = o.OrderStatus switch
                    {
                        1 => "待付款",
                        0 => "待付款",
                        2 => "配送中",
                        3 => "已完成",
                        _ => "未知"
                    },
                    SelectedPayment = o.SelectedPayment,
                    CreateDate = o.CreateDate,
                    ShippingFee = o.ShippingFee,
                    UsedPoints = o.UsedPoints,
                    DiscountAmount = o.DiscountAmount,
                    ShippingMethodName = _context.ShippingMethods
                        .Where(sm => sm.ShippingMethodId == o.ShippingMethodId)
                        .Select(sm => sm.Name)
                        .FirstOrDefault(),
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    ShippingAddress = o.ShippingAddress,
                    CouponCode = o.UserCoupon != null ? o.UserCoupon.Coupon.CouponCode : null,
                    CouponId = o.UserCoupon != null ? (int?)o.UserCoupon.Coupon.CouponId : null,
                    CouponDiscount = o.DiscountAmount - o.UsedPoints,
                    CouponDiscountType = o.UserCoupon != null ? o.UserCoupon.Coupon.DiscountType : null,
                    CouponDiscountValue = o.UserCoupon != null ? (decimal?)o.UserCoupon.Coupon.DiscountValue : null,
                    SelectedGateway = o.SelectedGateway,
                    SelectedPayment = o.SelectedPayment,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        OrderId = oi.OrderId,
                        Skuid = oi.Skuid,
                        ProductName = oi.Sku.Product.ProductName,
                        SkuName = oi.Sku.Skuname,
                        Quantity = oi.Quantity,
                        PriceAtPurchase = oi.PriceAtPurchase,
                        ImageUrl = oi.Sku.Product.ProductImages
                            .Where(pi => pi.IsMainOrNot == 1)
                            .Select(pi => pi.ImageUrl)
                            .FirstOrDefault() ?? oi.Sku.Product.ProductImages
                            .OrderBy(pi => pi.ImageSortBy)
                            .Select(pi => pi.ImageUrl)
                            .FirstOrDefault()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return order;
        }
    }
}
