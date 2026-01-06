using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Users.Interface;
using PcShop.Models;

namespace PcShop.Areas.Users.Data
{
    public class MeMberCenterData:IMemberCenterData
    {
        private readonly ExamContext _context;

        public MeMberCenterData(ExamContext context)
        {
            _context = context;
        }
        public Task<UserProfile?> GetUserAsync(int userId) //還沒寫Notracking版Getuserasync
        {
            return _context.UserProfiles
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public Task<List<Order>> GetLatestOrdersAsync(int userId, int take)
        {
            return _context.Orders
                .AsNoTracking() //有asnotracking就是不追蹤 不能拿去做update , 只能當GET用 
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreateDate)
                .Take(take)
                .ToListAsync();
        }

        public Task<UserProfile?> GetUserForUpdateAsync(int userId)
        {
            return _context.UserProfiles
                .FirstOrDefaultAsync(u => u.UserId == userId); // 不要 AsNoTracking
        }
        public async Task<bool> IsMailUsed(int userId , string mail)
        {
            return await _context.UserProfiles.AnyAsync(u => u.Mail == mail && u.UserId != userId); // 不要 AsNoTracking

        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<UserProfile?> GetUserByEmailTokenAsync(string token)
        {
            return _context.UserProfiles
                .FirstOrDefaultAsync(u => u.EmailVerifyToken == token);
        }

        public async Task<int> GetUserAvailablePointsAsync(int userId)
        {
            return await _context.GamePoints
                .Where(p => p.UserId == userId && p.Status == 1)
                .SumAsync(p => p.Points);
        }

    }
}

