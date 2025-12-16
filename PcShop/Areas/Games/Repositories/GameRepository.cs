using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Games.Repositories.Interfaces;
using PcShop.Models;
using System;

namespace PcShop.Areas.Games.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly ExamContext _context;

        public GameRepository(ExamContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetByIdAsync(int gameId)
        {

            return await _context.Games
                .Where(g => g.GameId == gameId && g.Status)
                .FirstOrDefaultAsync();
        }
    }

}
