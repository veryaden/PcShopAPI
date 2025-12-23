using PcShop.Models;
using static PcShop.Areas.Users.DTO.MemberCenterDTO;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IMemberCenterService
    {
        Task<MemberOverviewDto> GetOverviewAsync(int userId);
    }
}
