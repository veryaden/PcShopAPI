using PcShop.Models;

namespace PcShop.Areas.Users.Interface
{
    public interface IAuthData
    {
        UserProfile? GetUserByEmail(string email);
        UserProfile GetUserById(int userId);
        UserProfile InsertUser(UserProfile user);

       
        void Save();
    }
}
