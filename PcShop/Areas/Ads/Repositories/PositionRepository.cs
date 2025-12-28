using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Ads.Repositories.Interfaces;
using PcShop.Models;

namespace PcShop.Areas.Ads.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly ExamContext _context;
        public PositionRepository(ExamContext context) => _context = context;

        public async Task<List<Position>> GetActivePositionsAsync()
            => await _context.Positions
                .Where(p => p.IsActive == true)
                .OrderBy(p => p.PositionId)
                .ToListAsync();

        public Task<Position?> GetAsync(int positionId)
            => _context.Positions.FirstOrDefaultAsync(p => p.PositionId == positionId);
    }
}
