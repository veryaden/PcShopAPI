using PcShop.Areas.Users.DTO;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IAuthBus
    {
        Task<Object> GoogleLoginAsync(string idToken);

        void CompleteProfile(int userId, CompleteProfileRequestDTO dto);
        void Register(RegisterRequestDTO dto);
    }
}