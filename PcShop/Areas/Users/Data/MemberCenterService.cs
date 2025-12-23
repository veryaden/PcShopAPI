using Microsoft.IdentityModel.Tokens;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static PcShop.Areas.Users.DTO.MemberCenterDTO;
using static PcShop.Areas.Users.DTO.MemberCenterDTO.MemberLatestOrderDto;


namespace PcShop.Areas.Users.Data
{
    public class MemberCenterService : IMemberCenterService
    {
        private readonly IMemberCenterData _member;

        public MemberCenterService(IMemberCenterData member)
        {
            _member = member;
        }
        public async Task<MemberOverviewDto> GetOverviewAsync(int userId)
        {
            var user = await _member.GetUserAsync(userId)
                       ?? throw new Exception("使用者不存在");

            var orders = await _member.GetLatestOrdersAsync(userId, take: 3);

            return new MemberOverviewDto
            {
                Profile = new MemberProfileDto
                {
                    FullName = user.FullName ?? "",
                    BirthDate = user.BirthDay,
                    Phone = user.Phone ?? "",
                    Email = user.Mail ?? ""
                },
                LatestOrders = orders.Select(o =>
                {
                    var status = (OrderStatus)o.OrderStatus;

                    return new MemberLatestOrderDto
                    {
                        OrderNo = o.OrderNo,
                        TotalAmount = o.TotalAmount,
                        StatusText = GetStatusText(status),
                        StatusCode = GetStatusCode(status)
                    };
                }).ToList()
            };
        }

        private static string GetStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "待付款",
                OrderStatus.Shipping => "配送中",
                OrderStatus.Completed => "已完成",
                _ => "處理中"
            };
        }

        private static string GetStatusCode(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "pending",
                OrderStatus.Shipping => "shipping",
                OrderStatus.Completed => "completed",
                _ => "pending"
            };
        }
    }

}


