using PcShop.Areas.Ads.Dtos;

namespace PcShop.Areas.Ads.Repositories.Interfaces;

public interface IPositionRepository
{
    Task<List<PositionDto>> GetPositionsAsync();
    Task<int?> GetPositionIdByCodeAsync(string code);
}
