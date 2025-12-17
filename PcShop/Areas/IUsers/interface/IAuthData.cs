using PcShop.Models;

namespace PcShop.Areas.Users.Interface
{
    public interface IAuthData
    {
        Task <UserProfile>? GetUserByEmail(string email);
        Task<UserProfile> GetUserById(int userId);
        Task <UserProfile> InsertUser(UserProfile user);

       
        Task SaveAsync();
    }
}
