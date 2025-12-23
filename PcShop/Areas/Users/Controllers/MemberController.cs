using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Data;
using System.Security.Claims;
using static PcShop.Areas.Users.DTO.MemberCenterDTO;

namespace PcShop.Areas.Users.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/member")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberCenterService _service;

        public MemberController(IMemberCenterService service)
        {
            _service = service;
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

        [HttpGet("overview")]
        public async Task<IActionResult> Overview()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dto = await _service.GetOverviewAsync(userId);
            return Ok(dto);
        }
        [HttpGet("orders")]
        public async Task<IActionResult> Orders([FromQuery] OrderStatus? status)
        {
            int userId = GetUserIdOrThrow();
            return Ok(await _service.GetOrdersAsync(userId, status));
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
            await _service.ChangePasswordAsync(userId, dto);
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
    }
}
