using PcShop.Ads.Dtos;

namespace PcShop.Ads.Repositories.Interfaces;

public interface IAdRepository
{
    // Public
    Task<List<AdDto>> GetActiveAdsByPositionCodeAsync(string positionCode);
    Task TrackClickAsync(int adId, string positionCode);

    // Admin
    Task<List<AdDto>> AdminListAdsAsync();
    Task<int> AdminCreateAdAsync(AdUpsertDto dto);
    Task AdminUpdateAdAsync(int adId, AdUpsertDto dto);
    Task AdminDeleteAdAsync(int adId);
    Task<List<ReportRowDto>> AdminReportAsync(DateTime from, DateTime to);
}
