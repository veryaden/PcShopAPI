using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Ads.Dtos;
using PcShop.Areas.Ads.Repositories.Interfaces;
using PcShop.Areas.Ads.Services.Interfaces;
using PcShop.Models;

namespace PcShop.Areas.Ads.Services
{
    public class AdService : IAdService
    {
        private readonly IAdRepository _adRepo;
        private readonly IPositionRepository _posRepo;
        private readonly ExamContext _context;

        public AdService(IAdRepository adRepo, IPositionRepository posRepo, ExamContext context)
        {
            _adRepo = adRepo;
            _posRepo = posRepo;
            _context = context;
        }

        public async Task<List<AdDto>> GetAdsAsync(string positionCode)
        {
            var ads = await _adRepo.GetAdsByPositionCodeAsync(positionCode);

            return ads.Select(a => new AdDto
            {
                AdId = a.AdId,
                Title = a.Title,
                MediaUrl = a.MediaUrl,
                LinkUrl = a.LinkUrl,
                PositionCode = a.Position?.Code ?? positionCode,
                PositionName = a.Position?.Name,
                Size = a.Position?.Size
            }).ToList();
        }

        public async Task<List<PositionDto>> GetPositionsAsync()
        {
            var positions = await _posRepo.GetActivePositionsAsync();
            return positions.Select(p => new PositionDto
            {
                PositionID = p.PositionId,
                Code = p.Code,
                Name = p.Name,
                Size = p.Size,
                IsActive = p.IsActive
            }).ToList();
        }

        public async Task UpsertAsync(AdUpsertDto dto)
        {
            // ✅ 基本驗證（最少要有）
            if (string.IsNullOrWhiteSpace(dto.Title)) throw new Exception("Title required");
            if (string.IsNullOrWhiteSpace(dto.MediaUrl)) throw new Exception("MediaUrl required");

            // ✅ 檢查 PositionID（若有填）
            if (dto.PositionID != null)
            {
                var pos = await _posRepo.GetAsync(dto.PositionID.Value);
                if (pos == null || pos.IsActive != true)
                    throw new Exception("Invalid PositionID");
            }

            if (dto.AdId == null)
            {
                var ad = new Ad
                {
                    Title = dto.Title,
                    MediaUrl = dto.MediaUrl,
                    LinkUrl = dto.LinkUrl,
                    PositionId = dto.PositionID,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Status = dto.Status
                };

                await _adRepo.AddAsync(ad);
                await _adRepo.SaveAsync();
                return;
            }

            var existing = await _adRepo.GetAdAsync(dto.AdId.Value);
            if (existing == null) throw new Exception($"AdId {dto.AdId.Value} not found");

            existing.Title = dto.Title;
            existing.MediaUrl = dto.MediaUrl;
            existing.LinkUrl = dto.LinkUrl;
            existing.PositionId = dto.PositionID;
            existing.StartTime = dto.StartTime;
            existing.EndTime = dto.EndTime;
            existing.Status = dto.Status;

            await _adRepo.SaveAsync();
        }

        // ✅ AdsReport：每天一筆累計 Clicks（最簡、好用）
        public async Task TrackClickAsync(int adId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var row = await _context.AdsReports
                .FirstOrDefaultAsync(r => r.AdId == adId && r.Date == today);

            if (row == null)
            {
                _context.AdsReports.Add(new AdsReport
                {
                    AdId = adId,
                    Date = today,
                    Clicks = 1
                });
            }
            else
            {
                row.Clicks += 1;
            }

            await _context.SaveChangesAsync();
        }
    }

}
