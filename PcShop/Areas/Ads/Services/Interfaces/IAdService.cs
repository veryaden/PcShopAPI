using PcShop.Areas.Ads.Dtos;

namespace PcShop.Areas.Ads.Services.Interfaces;

public interface IAdService
{
    // public
    Task<List<PositionDto>> GetPositionsAsync();
    Task<List<AdDto>> GetSlotAsync(string positionCode);
    Task TrackClickAsync(TrackClickDto dto);

    // admin
    Task<List<AdDto>> AdminListAdsAsync();
    Task<int> AdminCreateAdAsync(AdUpsertDto dto);
    Task AdminUpdateAdAsync(int adId, AdUpsertDto dto);
    Task AdminDeleteAdAsync(int adId);
    Task<List<ReportRowDto>> AdminReportAsync(DateTime from, DateTime to);
    Task TrackClickAsync(int adId, string positionCode);
    
}
