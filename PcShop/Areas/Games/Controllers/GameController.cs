using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Games.Dtos;
using PcShop.Areas.Games.Services;
using PcShop.Domains.Enums;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace PcShop.Areas.Games.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }



        [Authorize]
        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore([FromBody] SubmitGameScoreDto dto)
        {
            if (dto == null)
                return BadRequest("DTO is null");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            await _gameService.SubmitScoreAsync(
                userId,
                dto.Score,
                dto.GameId
            );

            return Ok(new { success = true, userId, dto });
        }


    }
}

