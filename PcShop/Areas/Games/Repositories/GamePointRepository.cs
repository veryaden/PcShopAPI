using PcShop.Areas.Games.Repositories.Interfaces;
using PcShop.Models;
using System;

namespace PcShop.Areas.Games.Repositories
{
    public class GamePointRepository : IGamePointRepository
    {
        private readonly ExamContext _context;

        public GamePointRepository(ExamContext context)
        {
            _context = context;
        }

        public async Task AddAsync(GamePoint point)
        {
            _context.GamePoints.Add(point);
            await _context.SaveChangesAsync();
        }
    }

}
