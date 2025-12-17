using Microsoft.EntityFrameworkCore;
using PcShop.Models;

namespace PcShop.Areas.Users.Interface
{
    public interface IOAuthData
    {
        Task<Oauth> GetByProvider(string provider, string providerUserId);
        Task<Oauth> GetByUserIdAndProvider(int userId, string provider);

        Task<Oauth> GetByUserId(int userId);
        
        Task Insert(Oauth entity);
        Task SaveAsync();
    }
}
