using Google.Apis.Auth;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.DTO;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.Security.Claims;

namespace PcShop.Areas.Users.Data
{
    public class AuthServices: IAuthServices
    {
        private readonly IConfiguration _config;
        private readonly IAuthData _data;
        private readonly IJwtService _jwtService;
        private readonly IOAuthData _oauthData;
        public AuthServices(IConfiguration config, IAuthData data, IJwtService jwtService , IOAuthData oauthData)
        {
            _config = config;
            _data = data;
            _jwtService = jwtService;
            _oauthData = oauthData;
        }

        private AuthResponseDTO CreateAuthResponse(UserProfile user, string provider)
        {
            return new AuthResponseDTO
            {
                Token = _jwtService.GenerateToken(user),
                Message = "Login Success",
                User = new AuthUserDTO
                {
                    UserId = user.UserId,
                    Mail = user.Mail,
                    FullName = user.FullName,
                    ImageUrl = user.ImageUrl,
                    ProfileCompleted = user.ProfileCompleted,
                    Provider = provider
                }
            };
        }
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await ValidateLocalLogin(dto);
            return CreateAuthResponse(user, "Local");
        }

        public async Task<AuthResponseDTO> GoogleLoginAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _config["Google:ClientId"] }
                });

            var user = await HandleGoogleOAuthAsync(
                payload.Email,
                payload.Name,
                payload.Subject
            );

            return CreateAuthResponse(user, "Google");
        }


        public async Task CompleteProfileAsync(int userId, CompleteProfileRequestDTO dto)
        {
            var user = await _data.GetUserById(userId);
            if (user == null) throw new Exception("User not found");
            if (user.ProfileCompleted) throw new Exception("Profile already completed");

            user.Phone = dto.Phone;
            user.Address = dto.Address;
            user.ShippingAddress = dto.ShippingAddress;
            user.BirthDay = dto.BirthDay;

            user.ProfileCompleted = true;
            user.UpdateTime = DateTime.Now;

            await _data.SaveAsync();
        }

        public async Task RegisterAsync(RegisterRequestDTO dto)
        {
            if (await _data.GetUserByEmail(dto.Mail) != null)
                throw new Exception("Email already exists");

            var user = new UserProfile
            {
                FullName = dto.FullName,
                Mail = dto.Mail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                // [修正] 這裡要補上 DTO 傳進來的其他欄位
                Phone = dto.Phone,
                Address = dto.Address,
                ShippingAddress = dto.ShippingAddress,
                BirthDay = dto.BirthDay,

                ProfileCompleted = true, // 本地註冊通常是一次填完，所以設為 true
                CreateTime = DateTime.Now,
                Status = 1,
                ImageUrl = "/images/no-image.png",
                Provider = null // 標記為本地帳號
            };

            await _data.InsertUser(user);
            await _data.SaveAsync();
        }


        private async Task<UserProfile> HandleGoogleOAuthAsync(string email,string name,string googleSub)
        {
            const string provider = "Google";

            var oauth = await _oauthData.GetByProvider(provider, googleSub);

            if (oauth != null)
            {
                var user = await _data.GetUserById(oauth.UserId.Value);
                if (user == null)
                    throw new Exception("OAuth 綁定異常");

                oauth.LastLoginAt = DateTime.Now;
                await _oauthData.SaveAsync();

                return user;
            }

            // 尚未綁定
            var existingUser = await _data.GetUserByEmail(email);

            if (existingUser == null)
            {
                existingUser = new UserProfile
                {
                    Mail = email,
                    FullName = name,
                    ProfileCompleted = false,
                    ImageUrl = "/images/no-image.png",
                    CreateTime = DateTime.Now,
                    IsMailVerified = 1,
                    IsMailVerifiedTime = DateTime.Now,
                    Status = 1
                };

                await _data.InsertUser(existingUser);
                await _data.SaveAsync();
            }

            var newOauth = new Oauth
            {
                UserId = existingUser.UserId,
                Provider = provider,
                ProviderUserId = googleSub,
                CreatedAt = DateTime.Now,
                LastLoginAt = DateTime.Now
            };

            await _oauthData.Insert(newOauth);
            await _oauthData.SaveAsync();

            return existingUser;
        }
        private async Task<UserProfile> ValidateLocalLogin(LoginDTO dto)
        {
            // 1️ 用 Email 找使用者
            var user = await _data.GetUserByEmail(dto.Mail); 

            if (user == null)
                throw new Exception("帳號或密碼錯誤"); // 避免洩漏帳號是否存在

            // 2️ [修改這裡] 檢查是否為第三方帳號
            // 去 Oauth 表查這個 UserId 有沒有紀錄
            var oauthProfile = await _oauthData.GetByUserId(user.UserId);

            if (oauthProfile != null)
            {
                // 如果查到了，代表他是用 Google (或其他) 登入的
                // 視你的需求，如果你想「強制」他回去用 Google 登入，就丟出例外：
                throw new Exception($"此帳號已綁定 {oauthProfile.Provider}，請使用該方式登入");
            }

            // 3️⃣ 必須有密碼 (這行其實可以跟下面合併，但保留也沒關係)
            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new Exception("帳號或密碼錯誤");

            // 4️⃣ 驗證密碼
            bool ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok)
                throw new Exception("帳號或密碼錯誤");

            // 5️⃣ 狀態檢查
            if (user.Status == 0)
                throw new Exception("帳號已停用");

            return user;
        }


    }
}
