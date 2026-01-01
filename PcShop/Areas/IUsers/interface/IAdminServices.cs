using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Models;
using System.Linq.Dynamic.Core;
using static PcShop.Areas.Users.DTO.CompleteProfileRequestDTO;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IAdminServices
    {
        Task<OrderPagedResult<OrderListDTO>> GetOrderListAsync(OrderStatus? status, string? orderno, int page, int pageSize);

        Task<OrderDetailsDTO> GetOrderDetailAsync(int orderId);
        //Dashboard
        Task<AdminDashboardOverviewDto> GetOverviewAsync();
    }
}