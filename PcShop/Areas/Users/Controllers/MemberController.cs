using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using System.Security.Claims;
using static PcShop.Areas.Users.DTO.EmailDTO;
using static PcShop.Areas.Users.DTO.MemberCenterDTO;

namespace PcShop.Areas.Users.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/member")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberCenterService _service;
        private readonly IConfiguration _config;

        public MemberController(IMemberCenterService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        private int GetUserIdOrThrow()
        {
            // 常見：ClaimTypes.NameIdentifier (例如 "1")
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 有些 JWT 會把 user id 放在 "sub"
            if (string.IsNullOrEmpty(idStr))
                idStr = User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(idStr))
                throw new UnauthorizedAccessException("Token 缺少 userId claim (NameIdentifier/sub)");

            if (!int.TryParse(idStr, out var userId))
                throw new UnauthorizedAccessException($"Token userId 不是數字：{idStr}");

            return userId;
        }
        [Authorize]
        [HttpGet("overview")]
        public async Task<IActionResult> Overview()
        {
            int userId = GetUserIdOrThrow();
            var dto = await _service.GetOverviewAsync(userId);
            return Ok(dto);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = GetUserIdOrThrow();
            return Ok(await _service.GetProfileAsync(userId));
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] MemberProfileEditDto dto)
        {
            int userId = GetUserIdOrThrow();
            await _service.UpdateProfileAsync(userId, dto);
            return NoContent();
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<IActionResult> GetAddress()
        {
            int userId = GetUserIdOrThrow();
            return Ok(await _service.GetAddressAsync(userId));
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<IActionResult> UpdateAddress([FromBody] MemberAddressEditDto dto)
        {
            int userId = GetUserIdOrThrow();
            await _service.UpdateAddressAsync(userId, dto);
            return NoContent();
        }

        [Authorize]
        [HttpGet("security")]
        public async Task<IActionResult> GetSecurity()
        {
            int userId = GetUserIdOrThrow();
            return Ok(await _service.GetSecurityAsync(userId));
        }

        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            int userId = GetUserIdOrThrow();

            var result = await _service.ChangePasswordAsync(userId, dto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return NoContent();
        }

        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            int userId = GetUserIdOrThrow();
            var url = await _service.UploadAvatarAsync(userId, file);
            return Ok(new { imageUrl = url });
        }


        [Authorize]
        [HttpPost("verify-email/send")]
        public async Task<IActionResult> SendVerifyEmail([FromBody] SendVerifyEmailDto dto)
        {
            int userId = GetUserIdOrThrow();
            await _service.SendVerifyEmailAsync(userId, dto.FrontendUrl);
            return NoContent();
        }
        [AllowAnonymous]
        [HttpGet("verify-email/confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            await _service.ConfirmEmailAsync(token);
            return Ok(new { message = "Email 驗證成功" });
        }

        [Authorize]
        [HttpPut("email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto dto)
        {
            int userId = GetUserIdOrThrow();

            var frontendUrl = _config["FrontendUrl"];
            if (string.IsNullOrWhiteSpace(frontendUrl))
                throw new Exception("FrontendUrl 未設定");
            try
            {
                await _service.UpdateEmailAsync(userId, dto.NewEmail, frontendUrl);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("points")]
        public async Task<IActionResult> GetMyPoints()
        {
            int userId = GetUserIdOrThrow();
            var points = await _service.GetMyAvailablePointsAsync(userId);

            return Ok(new
            {
                points
            });
        }
    }
}
