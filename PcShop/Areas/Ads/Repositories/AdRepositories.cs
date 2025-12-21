using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Ads.Repositories.Interfaces;
using PcShop.Models;

namespace PcShop.Areas.Ads.Repositories
{
    public class AdRepository : IAdRepository
    {
        private readonly ExamContext _context;

        public AdRepository(ExamContext context)
        {
            _context = context;
        }

        public async Task<List<Ad>> GetAdsByPositionCodeAsync(string code)
        {
            var now = DateTime.Now;

            return await _context.Ads
                .Include(a => a.Position)
                .Where(a =>
                    a.Status &&
                    a.Position != null &&
                    a.Position.Code == code &&
                    a.Position.IsActive == true &&
                    (a.StartTime == null || a.StartTime <= now) &&
                    (a.EndTime == null || a.EndTime >= now)
                )
                .OrderBy(a => a.AdId)
                .ToListAsync();
        }

        public Task<Ad?> GetAdAsync(int adId)
            => _context.Ads.FirstOrDefaultAsync(a => a.AdId == adId);

        public async Task AddAsync(Ad ad)
            => await _context.Ads.AddAsync(ad);

        public Task SaveAsync()
            => _context.SaveChangesAsync();
    }
}
