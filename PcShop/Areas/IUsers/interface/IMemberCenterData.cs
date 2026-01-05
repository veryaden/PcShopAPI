using PcShop.Models;

namespace PcShop.Areas.Users.Interface
{
    public interface IMemberCenterData
    {
        Task<UserProfile?> GetUserAsync(int userId);
        Task<List<Order>> GetLatestOrdersAsync(int userId, int take);
        Task<UserProfile?> GetUserForUpdateAsync(int userId);
        Task<bool> IsMailUsed(int userId, string mail);
        Task SaveAsync();

        Task<UserProfile?> GetUserByEmailTokenAsync(string token);
        Task<int> GetUserAvailablePointsAsync(int userId);
    }
}
