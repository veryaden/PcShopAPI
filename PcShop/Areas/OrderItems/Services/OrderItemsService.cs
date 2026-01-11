using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using PcShop.Areas.OrderItems.Dtos;
using PcShop.Areas.OrderItems.Repositories;
using PcShop.Areas.Users.Data;
using PcShop.Models;

namespace PcShop.Areas.OrderItems.Services
{
    public class OrderItemsService : IOrderItemsService
    {
        private readonly ExamContext _context;
        private IOrderService _order;

        public OrderItemsService(ExamContext context, IOrderService order)
        {
            _context = context;
            _order = order;
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId, int userId)
        {
            //var items = await _context.OrderItems
            //    .Where(oi => oi.OrderId == orderId && oi.Order.UserId == userId)
            //    .Select(oi => new Users.DTO.OrderItemDto
            //    {
            //        OrderItemId = oi.OrderItemId,
            //        OrderId = oi.OrderId,
            //        Skuid = oi.Skuid,
            //        ProductName = oi.Sku.Product.ProductName,
            //        SkuName = oi.Sku.Skuname,
            //        Quantity = oi.Quantity,
            //        PriceAtPurchase = oi.PriceAtPurchase,
            //        ImageUrl = oi.Sku.Product.ProductImages
            //            .Where(pi => pi.IsMainOrNot == 1)
            //            .Select(pi => pi.ImageUrl)
            //            .FirstOrDefault() ?? oi.Sku.Product.ProductImages
            //            .OrderBy(pi => pi.ImageSortBy)
            //            .Select(pi => pi.ImageUrl)
            //            .FirstOrDefault()
            //    })
            //    .ToListAsync();

            return null;
        }

        //public async Task<OrderDetailDto> GetOrderDetailAsync(int orderId, int userId)
        //{
        //    var order = await _context.Orders
        //        .Where(o => o.OrderId == orderId && o.UserId == userId)
        //        .Select(o => new OrderDetailDto
        //        {
        //            OrderId = o.OrderId,
        //            OrderNo = o.OrderNo,
        //            TotalAmount = o.TotalAmount,
        //            OrderStatus = o.OrderStatus,
        //            StatusName = o.OrderStatus == 1 ? "待付款" :
        //                 o.OrderStatus == 0 ? "待付款" :
        //                 o.OrderStatus == 2 ? "配送中" :
        //                 o.OrderStatus == 3 ? "已完成" : "未知",
        //            SelectedPayment = o.SelectedPayment,
        //            CreateDate = o.CreateDate,
        //            ShippingFee = o.ShippingFee,
        //            UsedPoints = o.UsedPoints,
        //            DiscountAmount = o.DiscountAmount,
        //            ShippingMethodName = _context.ShippingMethods
        //                .Where(sm => sm.ShippingMethodId == o.ShippingMethodId)
        //                .Select(sm => sm.Name)
        //                .FirstOrDefault(),
        //            ReceiverName = o.ReceiverName,
        //            ReceiverPhone = o.ReceiverPhone,
        //            ShippingAddress = o.ShippingAddress,
        //            CouponCode = o.UserCoupon != null ? o.UserCoupon.Coupon.CouponCode : null,
        //            CouponId = o.UserCoupon != null ? (int?)o.UserCoupon.Coupon.CouponId : null,
        //            CouponDiscount = o.DiscountAmount - o.UsedPoints,
        //            CouponDiscountType = o.UserCoupon != null ? o.UserCoupon.Coupon.DiscountType : null,
        //            CouponDiscountValue = o.UserCoupon != null ? (decimal?)o.UserCoupon.Coupon.DiscountValue : null,
        //            SelectedGateway = o.SelectedGateway,
        //            Items = o.OrderItems.Select(oi => new OrderItemDto
        //            {
        //                OrderItemId = oi.OrderItemId,
        //                OrderId = oi.OrderId,
        //                Skuid = oi.Skuid,
        //                ProductName = oi.Sku.Product.ProductName,
        //                SkuName = oi.Sku.Skuname,
        //                Quantity = oi.Quantity,
        //                PriceAtPurchase = oi.PriceAtPurchase,
        //                ImageUrl = oi.Sku.Product.ProductImages
        //                    .Where(pi => pi.IsMainOrNot == 1)
        //                    .Select(pi => pi.ImageUrl)
        //                    .FirstOrDefault() ?? oi.Sku.Product.ProductImages
        //                    .OrderBy(pi => pi.ImageSortBy)
        //                    .Select(pi => pi.ImageUrl)
        //                    .FirstOrDefault()
        //            }).ToList()
        //        })
        //        .FirstOrDefaultAsync();

        //    return order;

        //}

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var query = await _context.Orders.Where(o => o.UserId == userId && o.OrderId == orderId).FirstOrDefaultAsync();

            if(query != null) 
            {
                query.OrderStatus = 3;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
          
        }

        public async Task<OrderDetailsDTO> GetOrderDetailAsync(int orderId, int userId)
        {

            var order = await _order.GetOrderDetailAsync(orderId, userId)
               ?? throw new Exception("訂單不存在");

            return new OrderDetailsDTO
            {
                OrderId = order.OrderId,
                OrderNo = order.OrderNo,
                Status = ((OrderStatus)order.OrderStatus).ToString(),
                CreateDate = order.CreateDate,
                TotalAmount = order.TotalAmount,
                ShippingFee = order.ShippingFee,
                DiscointAmount = order.DiscountAmount,
                UsedPoints = order.UsedPoints,
                ReceiverPhone = order.ReceiverPhone,
                ReceiverName = order.User.FullName,
                ShippingAddress = order.ShippingAddress,
                ShippingMethodName = order.ShippingMethod.Name,
                SelectedPayment = order.SelectedPayment,
                Items = order.OrderItems.Select(oi =>
                {
                    var product = oi.Sku.Product;
                    var image = product.ProductImages
                        //.Where(img => img.IsMain)          // ⭐ 主圖
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault();

                    return new OrderItemDto
                    {
                        ProductName = product.ProductName,
                        ProductImage = image,              // ⭐ 圖片來源
                        Quantity = oi.Quantity,
                        UnitPriceAtPurchase = oi.PriceAtPurchase
                    };
                }).ToList()
            };
        }

        Task<IEnumerable<Dtos.OrderItemDto>> IOrderItemsService.GetOrderItemsAsync(int orderId, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
