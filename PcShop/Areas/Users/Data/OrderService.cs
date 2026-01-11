using Google.Apis.Auth;
using Humanizer;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using Org.BouncyCastle.Security;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.DTO;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Net.Mail;
using System.Security.Claims;


namespace PcShop.Areas.Users.Data
{
    public class OrderService : IOrderService
    {
        private IOrderData _order;

        public OrderService(IOrderData order)
        {
            _order = order;
        }
        public async Task<OrderPagedResult<OrderListDTO>> GetOrderListAsync(int userId,OrderStatus? status, string? orderno, int page, int pageSize)
        {
            var pagedOrders = await _order.GetOrdersAsync(userId, status, orderno, page, pageSize);

            return new OrderPagedResult<OrderListDTO>
            {
                Page = pagedOrders.Page,
                PageSize = pagedOrders.PageSize,
                Total = pagedOrders.Total,
                Items = pagedOrders.Items.Select(o => new OrderListDTO
                {
                    OrderId = o.OrderId,
                    OrderNo = o.OrderNo,
                    CreateDate = o.CreateDate,
                    TotalAmount = o.TotalAmount,
                    Status = (OrderStatus)o.OrderStatus
                }).ToList()
            };
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
                        UnitPriceAtPurchase= oi.PriceAtPurchase
                    };
                }).ToList()
            };
        }
    }
}
