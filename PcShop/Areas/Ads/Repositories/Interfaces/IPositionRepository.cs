using PcShop.Models;

namespace PcShop.Areas.Ads.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        Task<List<Position>> GetActivePositionsAsync();
        Task<Position?> GetAsync(int positionId);
    }
}
