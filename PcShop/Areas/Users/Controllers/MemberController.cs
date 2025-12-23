using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.IUsers.Interface;
using System.Security.Claims;

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

        [HttpGet("overview")]
        public async Task<IActionResult> Overview()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dto = await _service.GetOverviewAsync(userId);
            return Ok(dto);
        }
    }
}
