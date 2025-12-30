using Microsoft.EntityFrameworkCore;
using PcShop.Ads.Dtos;
using PcShop.Ads.Repositories.Interfaces;
using PcShop.Models;
using System.Data.SqlClient;

namespace PcShop.Ads.Repositories;

public class AdRepository : IAdRepository
{
    private readonly ExamContext _context;

    public AdRepository(ExamContext context)
    {
        _context = context;
    }

    // ============ Public ============

    public async Task<List<AdDto>> GetActiveAdsByPositionCodeAsync(string positionCode)
    {
        var now = DateTime.Now;

        // Ad.PositionId 是 int?，所以 join 要小心：只取有 PositionId 的
        var query =
            from a in _context.Ads
            where a.PositionId != null
            join p in _context.Positions on a.PositionId equals p.PositionId
            where p.Code == positionCode
                  && a.Status == true
                  && (a.StartTime == null || a.StartTime <= now)
                  && (a.EndTime == null || a.EndTime >= now)
            orderby a.AdId descending
            select new AdDto
            {
                AdId = a.AdId,
                Title = a.Title,
                MediaUrl = a.MediaUrl,

                // 你的 Model LinkUrl 非 nullable，但 DB 可能允許 NULL，
                // 為保險：DTO 用 string?，這邊做 null/空字串兼容
                LinkUrl = string.IsNullOrWhiteSpace(a.LinkUrl) ? null : a.LinkUrl,

                PositionId = a.PositionId ?? 0,
                PositionCode = p.Code,
                Type = a.Type,
                Status = a.Status,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            };

        return await query.ToListAsync();
    }

    public async Task TrackClickAsync(int adId, string positionCode)
    {
        // 1) 先找 PositionId（PositionId 在 AdsReport 是 int?）
        var positionId = await _context.Positions
            .Where(p => p.Code == positionCode)
            .Select(p => (int?)p.PositionId)
            .FirstOrDefaultAsync();

        if (positionId == null) return;

        // 2) DateOnly 的今天
        var today = DateOnly.FromDateTime(DateTime.Today);

        // 3) 找今天的彙總列（AdsReport.PositionId 是 int?）
        var row = await _context.AdsReports
            .FirstOrDefaultAsync(r =>
                r.AdId == adId &&
                r.Date == today &&
                r.PositionId == positionId
            );

        if (row == null)
        {
            row = new AdsReport
            {
                AdId = adId,
                PositionId = positionId,      // int?
                Date = today,                 // DateOnly
                Clicks = 1,
                CreatedAt = today             // DateOnly?
            };
            _context.AdsReports.Add(row);
        }
        else
        {
            row.Clicks += 1;
        }

        await _context.SaveChangesAsync();
    }

    // ============ Admin ============

    public async Task<List<AdDto>> AdminListAdsAsync()
    {
        // 讓沒有 PositionId 的廣告也能列出：用 left join 的寫法
        var query =
            from a in _context.Ads
            join p in _context.Positions on a.PositionId equals (int?)p.PositionId into gj
            from p in gj.DefaultIfEmpty()
            orderby a.AdId descending
            select new AdDto
            {
                AdId = a.AdId,
                Title = a.Title,
                MediaUrl = a.MediaUrl,
                LinkUrl = string.IsNullOrWhiteSpace(a.LinkUrl) ? null : a.LinkUrl,
                PositionId = a.PositionId ?? 0,
                PositionCode = p != null ? p.Code : "",
                Type = a.Type,
                Status = a.Status,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            };

        return await query.ToListAsync();
    }

    public async Task<int> AdminCreateAdAsync(AdUpsertDto dto)
    {
        var entity = new Ad
        {
            Title = dto.Title,
            MediaUrl = dto.MediaUrl,

            // Model 是 string 非 nullable，避免 null 塞進去
            LinkUrl = dto.LinkUrl ?? "",

            PositionId = dto.PositionId == 0 ? null : dto.PositionId, // 你前端若傳 0 表示未指定
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = dto.Status,
            Type = dto.Type,
            CreatedAt = DateOnly.FromDateTime(DateTime.Today)
        };

        _context.Ads.Add(entity);
        await _context.SaveChangesAsync();
        return entity.AdId;
    }

    public async Task AdminUpdateAdAsync(int adId, AdUpsertDto dto)
    {
        var entity = await _context.Ads.FirstOrDefaultAsync(x => x.AdId == adId);
        if (entity == null) return;

        entity.Title = dto.Title;
        entity.MediaUrl = dto.MediaUrl;
        entity.LinkUrl = dto.LinkUrl ?? "";
        entity.PositionId = dto.PositionId == 0 ? null : dto.PositionId;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.Status = dto.Status;
        entity.Type = dto.Type;

        await _context.SaveChangesAsync();
    }

    public async Task AdminDeleteAdAsync(int adId)
    {
        var entity = await _context.Ads.FirstOrDefaultAsync(x => x.AdId == adId);
        if (entity == null) return;

        _context.Ads.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ReportRowDto>> AdminReportAsync(DateTime from, DateTime to)
    {
        var start = DateOnly.FromDateTime(from.Date);
        var end = DateOnly.FromDateTime(to.Date);

        var query =
            from r in _context.AdsReports
            join a in _context.Ads on r.AdId equals a.AdId
            join p in _context.Positions on r.PositionId equals (int?)p.PositionId into gj
            from p in gj.DefaultIfEmpty()
            where r.Date >= start && r.Date <= end
            group new { r, a, p } by new
            {
                r.Date,
                PositionCode = (p != null ? p.Code : ""),
                r.AdId,
                a.Title
            }
            into g
            orderby g.Key.Date descending, g.Key.PositionCode, g.Key.AdId
            select new ReportRowDto
            {
                Date = g.Key.Date.ToDateTime(TimeOnly.MinValue), // DTO 若用 DateTime 給前端好處理
                PositionCode = g.Key.PositionCode,
                AdId = g.Key.AdId,
                Title = g.Key.Title,
                Clicks = g.Sum(x => x.r.Clicks)
            };

        return await query.ToListAsync();
    }
}
