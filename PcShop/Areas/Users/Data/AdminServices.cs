using Google.Apis.Auth;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.DTO;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using MailKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using Org.BouncyCastle.Security;


namespace PcShop.Areas.Users.Data
{
    public class AdminServices : IAdminServices
    {
        private IAdminData _admin;

        public AdminServices(IAdminData admin)
        {
            _admin = admin;
        }

        public async Task<OrderPagedResult<OrderListDTO>> GetOrderListAsync(OrderStatus? status , string? orderno, int page,int pageSize)
        {
            var pagedOrders = await _admin.GetOrdersAsync(status, orderno, page, pageSize);

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


        public async Task<OrderDetailsDTO> GetOrderDetailAsync(int orderId)
        {
            var order = await _admin.GetOrderDetailAsync(orderId)
                ?? throw new Exception("訂單不存在");

            return new OrderDetailsDTO
            {
                OrderId = order.OrderId,
                OrderNo = order.OrderNo,
                Status = ((OrderStatus)order.OrderStatus).ToString(),
                CreateDate = order.CreateDate,
                TotalAmount = order.TotalAmount,

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
