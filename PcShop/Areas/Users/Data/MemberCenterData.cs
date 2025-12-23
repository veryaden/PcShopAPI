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
        public Task<UserProfile?> GetUserAsync(int userId)
        {
            return _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public Task<List<Order>> GetLatestOrdersAsync(int userId, int take)
        {
            return _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreateDate)
                .Take(take)
                .ToListAsync();
        }

    }
}

