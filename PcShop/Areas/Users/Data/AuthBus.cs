using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.Security.Claims;
using PcShop.Areas.Users.DTO;

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
                   Audience = new[] { _config["Google:ClientId"] }
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
                    user.Provider,
                }
            };
        }

        public void CompleteProfile(int userId, CompleteProfileRequestDTO dto)
        {
            var user = _data.GetUserById(userId);
            if (user == null)
                throw new Exception("User not found");

            if (user.ProfileCompleted)
                throw new Exception("Profile already completed");

            // ⭐ 這裡只處理 DTO 裡「存在的欄位」
            user.Phone = dto.Phone;
            user.Address = dto.Address;
            user.ShippingAddress = dto.ShippingAddress;
            user.BirthDay = dto.BirthDay;

            user.ProfileCompleted = true;
            user.UpdateTime = DateTime.UtcNow;
            _data.Save();
        }
        public void Register(RegisterRequestDTO dto)
        {
            // ❗ 防呆：信箱不能重複
            var exists = _data.GetUserByEmail(dto.Mail);
            if (exists != null)
            {
                throw new Exception("Email already exists");
            }

            var user = new UserProfile
            {
                FullName = dto.FullName,
                Mail = dto.Mail,
                Phone = dto.Phone,
                Address = dto.Address,
                ShippingAddress = dto.ShippingAddress,
                BirthDay = dto.BirthDay,

                Provider = null,              // ⭐ 一般註冊
                ProfileCompleted = true,
                CreateTime = DateTime.UtcNow,
                Status = 1,                   // 使用中
                ImageUrl = "/images/no-image.png"
            };

            _data.InsertUser(user);
            _data.Save();
        }


    }
}
