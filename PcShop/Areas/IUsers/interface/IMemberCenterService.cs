using PcShop.Areas.Users.Data;
using PcShop.Models;
using System.Threading.Tasks;
using static PcShop.Areas.Users.Data.StatusCodeService;
using static PcShop.Areas.Users.DTO.MemberCenterDTO;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IMemberCenterService
    {
        Task<MemberOverviewDto> GetOverviewAsync(int userId);
        Task<MemberProfileEditDto> GetProfileAsync(int userId);
        Task UpdateProfileAsync(int userId, MemberProfileEditDto dto);
        Task<MemberAddressEditDto> GetAddressAsync(int userId);
        Task UpdateAddressAsync(int userId, MemberAddressEditDto dto);
        Task<AccountSecurityDto> GetSecurityAsync(int userId);
        Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto dto);

        Task UpdateEmailAsync(int userId, string newEmail, string frontendUrl);
        Task<string> UploadAvatarAsync(int userId, IFormFile file);
        Task SendVerifyEmailAsync(int userId, string frontendUrl);
        Task ConfirmEmailAsync(string token);

    }
}
