using PcShop.Ads.Dtos;

namespace PcShop.Ads.Repositories.Interfaces;

public interface IPositionRepository
{
    Task<List<PositionDto>> GetPositionsAsync();
    Task<int?> GetPositionIdByCodeAsync(string code);
}
