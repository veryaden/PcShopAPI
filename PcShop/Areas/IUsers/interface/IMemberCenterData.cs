using PcShop.Models;

namespace PcShop.Areas.Users.Interface
{
    public interface IMemberCenterData
    {
        Task<UserProfile?> GetUserAsync(int userId);
        Task<List<Order>> GetLatestOrdersAsync(int userId, int take);
        Task<UserProfile?> GetUserForUpdateAsync(int userId);
        Task SaveAsync();
    }
}
