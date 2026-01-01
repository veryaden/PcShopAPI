using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Models;
using static PcShop.Areas.Users.DTO.CompleteProfileRequestDTO;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IAdminData
    {
        Task<OrderPagedResult<Order>> GetOrdersAsync(OrderStatus? status, string? orderno, int page, int pageSize);

        Task<Order?> GetOrderDetailAsync(int orderId);

        //Dashboard
        Task<int> GetTotalMembersAsync();
        Task<decimal> GetYearlyRevenueAsync(int year);
        Task<int> GetMonthOrdersAsync(int year, int month);
        Task<decimal> GetAvgOrderAmountAsync();
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int year);
    }
}