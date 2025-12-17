using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Users.Interface;
using PcShop.Models;

namespace PcShop.Areas.Users.Data
{
    public class OAuthData : IOAuthData
    {
        private readonly ExamContext _context;

        public OAuthData(ExamContext context)
        {
            _context = context;
        }

        public async Task<Oauth> GetByProvider(string provider, string providerUserId)
        {
            return await _context.Oauths
                .FirstOrDefaultAsync(x => x.Provider == provider && x.ProviderUserId == providerUserId);
        }

        public async Task<Oauth>GetByUserIdAndProvider(int userId, string provider)
        {
            return await _context.Oauths
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Provider == provider);
        }
        public async Task<Oauth> GetByUserId(int userId)
        {
            // 假設你的 Context 叫 _context
            return await _context.Oauths.FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task Insert(Oauth entity)
        {
           await _context.Oauths.AddAsync(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
