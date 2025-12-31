using PcShop.Areas.Users.DTO;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IAuthBus
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);

        Task<AuthResponseDTO> GoogleLoginAsync(string idToken);

        Task CompleteProfileAsync(int userId, CompleteProfileRequestDTO dto);
        Task RegisterAsync(RegisterRequestDTO dto);
    }
}