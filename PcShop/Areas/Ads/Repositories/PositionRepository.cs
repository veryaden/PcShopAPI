using Microsoft.EntityFrameworkCore;
using PcShop.Ads.Dtos;
using PcShop.Ads.Repositories.Interfaces;
using PcShop.Models;
using System.Data.SqlClient;

namespace PcShop.Ads.Repositories;


public class PositionRepository : IPositionRepository
{
    private readonly ExamContext _context;

    public PositionRepository(ExamContext context)
    {
        _context = context;
    }

    public async Task<List<PositionDto>> GetPositionsAsync()
    {
        return await _context.Positions
            .OrderBy(p => p.PositionId)
            .Select(p => new PositionDto
            {
                PositionId = p.PositionId,
                Code = p.Code,
                Name = p.Name,
                Size = p.Size,
                IsActive = p.IsActive ?? false
            })
            .ToListAsync();
    }

    public async Task<int?> GetPositionIdByCodeAsync(string code)
    {
        return await _context.Positions
            .Where(p => p.Code == code)
            .Select(p => (int?)p.PositionId)
            .FirstOrDefaultAsync();
    }
}
