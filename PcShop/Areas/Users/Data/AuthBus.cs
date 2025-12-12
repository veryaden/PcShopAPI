using Google.Apis.Auth;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Interface;
using PcShop.Models;

namespace PcShop.Areas.Users.Data
{
    public class AuthBus: IAuthBus
    {
        private readonly IConfiguration _config;
        private readonly IAuthData _data;
        private readonly IJwtService _jwtService;

        public AuthBus(IConfiguration config, IAuthData data, IJwtService jwtService)
        {
            _config = config;
            _data = data;
            _jwtService = jwtService;
        }

        public async Task<object> GoogleLoginAsync(string idToken)
        {
            // 1. 驗證 ID Token
            string clientId = _config["Google:ClientId"];

            var payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });

            string email = payload.Email;
            string name = payload.Name;
            string avatar = payload.Picture;

            // 2. 查詢是否已有帳號
            var user = _data.GetUserByEmail(email);

            if (user == null)
            {
                // 3. 自動註冊
                user = new UserProfile
                {
                    Mail = email,
                    FullName = name,
                    Provider = "Google",
                    ProfileCompleted = false,
                    ImageUrl = "/Areas/Iusers/images/No_images.png",
                    CreateTime = DateTime.UtcNow
                };

                _data.InsertUser(user);
                _data.Save();
            }

            // 4. JWT
            string token = _jwtService.GenerateToken(user);

            return new
            {
                Message = "Google Login Success",
                Token = token,
                User = new
                {
                    user.UserId,
                    user.Mail,
                    user.FullName,
                    user.ImageUrl,
                    user.ProfileCompleted,
                }
            };
        }
    }
}
