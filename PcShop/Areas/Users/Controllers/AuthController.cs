using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.IUsers.Interface; // 修正命名空間
using PcShop.Areas.Users.DTO;
using System.Security.Claims;

namespace PcShop.Areas.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _bus;

        public AuthController(IAuthServices bus)
        {
            _bus = bus;
        }
        private int GetUserIdOrThrow()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(idStr))
                idStr = User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(idStr))
                throw new UnauthorizedAccessException("Token 缺少 userId claim");

            if (!int.TryParse(idStr, out var userId))
                throw new UnauthorizedAccessException("userId 非數字");

            return userId;
        }

        // 需求 3: 普通使用者登入
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var result = await _bus.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 需求 1 & 2: Google 登入 (檢測註冊/自動註冊)
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                // Service 會回傳 AuthResponseDTO，裡面包含 ProfileCompleted
                // 前端根據 ProfileCompleted 來決定要跳轉首頁還是填寫資料頁
                var result = await _bus.GoogleLoginAsync(request.IdToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 需求 1 的後半段: Google 用戶填寫完資料後呼叫
        [Authorize]
        [HttpPost("complete-profile")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequestDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // 必須使用 async/await
                int userId = GetUserIdOrThrow(); // ⭐ 用同一套邏輯

                await _bus.CompleteProfileAsync(userId, dto);
                return Ok(new { message = "資料更新成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 需求 4 的後半段: 本地新用戶註冊
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // 必須使用 async/await
                await _bus.RegisterAsync(dto);
                return Ok(new { message = "註冊成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDTO dto)
        {
            await _bus.ForgotPasswordAsync(dto.Mail);
            return Ok(new { message = "如果帳號存在，已寄出信件" });
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            try
            {
                await _bus.ResetPasswordAsync(dto.Token, dto.NewPassword);
                return Ok(new { message = "密碼重設成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                await _bus.VerifyEmailAsync(token);
                return Ok(new { message = "信箱驗證成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }

    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
    }
}