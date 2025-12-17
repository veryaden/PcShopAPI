using Microsoft.IdentityModel.Tokens;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace PcShop.Areas.Users.Data
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(UserProfile user)
        {   
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Mail),
                new Claim("name", user.FullName ?? "")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // 在 JwtService 中新增這個方法
        public string GenerateRegisterToken(string email, string provider, string providerKey, string name)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.Name, name),
                    // 這些是關鍵，把第三方資訊藏在 Token 裡
                    new Claim("Provider", provider),
                    new Claim("ProviderKey", providerKey),
                    // 標記這個 Token 的用途是 "Registration"
                    new Claim("TokenType", "Register")
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 設定有效期短一點，例如 10 分鐘，只讓他用來註冊
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

