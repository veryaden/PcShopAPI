using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.DTO;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.Security.Claims;

namespace PcShop.Areas.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthBus _bus;
        private readonly ExamContext _context; 

        public AuthController(IConfiguration config , IAuthBus bus , ExamContext context)
        {
            _config = config;
            _bus = bus;
            _context = context;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {

            var result = await _bus.GoogleLoginAsync(request.IdToken);

            return Ok(result);

        }
        [Authorize]
        [HttpPost("complete-profile")]
        public IActionResult CompleteProfile([FromBody] CompleteProfileRequestDTO dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            _bus.CompleteProfile(userId, dto);

            return Ok();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDTO dto)
        {
            _bus.Register(dto);
            return Ok();
        }


    }

    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
    }
}