using PcShop.Models;

namespace PcShop.Areas.Games.Repositories.Interfaces
{
    public interface IGameRepository
    {
        Task<Game?> GetByIdAsync(int gameId);
    }

}
