using PcShop.Models;

namespace PcShop.Areas.Games.Repositories.Interfaces
{
    public interface IGamePointRepository
    {
        Task AddAsync(GamePoint point);
    }

}
