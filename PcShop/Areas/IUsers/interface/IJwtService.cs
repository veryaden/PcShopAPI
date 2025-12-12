using PcShop.Models;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IJwtService
    {
        string GenerateToken(UserProfile user);
    }
}
