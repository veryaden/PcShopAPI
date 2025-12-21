using PcShop.Areas.Users.DTO;

namespace PcShop.Areas.IUsers.Interface
{
    public interface IAuthServices
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);

        Task<AuthResponseDTO> GoogleLoginAsync(string idToken);

        Task CompleteProfileAsync(int userId, CompleteProfileRequestDTO dto);
        Task RegisterAsync(RegisterRequestDTO dto);

        Task ForgotPasswordAsync(string mail);
        Task ResetPasswordAsync(string token, string newPassword);
        Task VerifyEmailAsync(string token);
    }
}