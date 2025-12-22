using PcShop.Models;

namespace PcShop.Areas.Ads.Repositories.Interfaces
{
    public interface IAdRepository
    {
        Task<List<Ad>> GetAdsByPositionCodeAsync(string code);
        Task<Ad?> GetAdAsync(int adId);
        Task AddAsync(Ad ad);
        Task SaveAsync();
    }
}
