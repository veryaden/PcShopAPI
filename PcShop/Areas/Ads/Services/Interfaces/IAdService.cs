using PcShop.Areas.Ads.Dtos;

namespace PcShop.Areas.Ads.Services.Interfaces
{
    public interface IAdService
    {
        Task<List<AdDto>> GetAdsAsync(string positionCode);
        Task UpsertAsync(AdUpsertDto dto);
        Task<List<PositionDto>> GetPositionsAsync();
        Task TrackClickAsync(int adId);
    }
}
