using PcShop.Areas.Ads.Dtos;
using PcShop.Areas.Ads.Repositories.Interfaces;
using PcShop.Areas.Ads.Services.Interfaces;

namespace PcShop.Areas.Ads.Services;

public class AdService : IAdService
{
    private readonly IAdRepository _adRepo;
    private readonly IPositionRepository _posRepo;

    public AdService(IAdRepository adRepo, IPositionRepository posRepo)
    {
        _adRepo = adRepo;
        _posRepo = posRepo;
    }

    public Task<List<PositionDto>> GetPositionsAsync()
        => _posRepo.GetPositionsAsync();

    public Task<List<AdDto>> GetSlotAsync(string positionCode)
        => _adRepo.GetActiveAdsByPositionCodeAsync(positionCode);

    public Task TrackClickAsync(TrackClickDto dto)
        => _adRepo.TrackClickAsync(dto.AdId, dto.PositionCode);

    public Task<List<AdDto>> AdminListAdsAsync()
        => _adRepo.AdminListAdsAsync();

    public Task<int> AdminCreateAdAsync(AdUpsertDto dto)
        => _adRepo.AdminCreateAdAsync(dto);

    public Task AdminUpdateAdAsync(int adId, AdUpsertDto dto)
        => _adRepo.AdminUpdateAdAsync(adId, dto);

    public Task AdminDeleteAdAsync(int adId)
        => _adRepo.AdminDeleteAdAsync(adId);

    public Task<List<ReportRowDto>> AdminReportAsync(DateTime from, DateTime to)
        => _adRepo.AdminReportAsync(from, to);
    public async Task TrackClickAsync(int adId, string positionCode)
    {
        await _adRepo.IncrementClickAsync(adId, positionCode);
    }
}
