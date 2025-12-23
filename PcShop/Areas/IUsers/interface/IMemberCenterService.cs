using PcShop.Areas.Users.Data;
using PcShop.Models;
using System.Threading.Tasks;
using static PcShop.Areas.Users.DTO.MemberCenterDTO;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IMemberCenterService
    {
        Task<MemberOverviewDto> GetOverviewAsync(int userId);
        Task<List<MemberOrderListDto>> GetOrdersAsync(int userId,OrderStatus? status);
        Task<MemberProfileEditDto> GetProfileAsync(int userId);
        Task UpdateProfileAsync(int userId, MemberProfileEditDto dto);
        Task<MemberAddressEditDto> GetAddressAsync(int userId);
        Task UpdateAddressAsync(int userId, MemberAddressEditDto dto);
        Task<AccountSecurityDto> GetSecurityAsync(int userId);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);

        Task<string> UploadAvatarAsync(int userId, IFormFile file);
    }
}
