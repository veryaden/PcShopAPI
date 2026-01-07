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

            // ✅ 穩定抓 userId（同時支援 NameIdentifier / sub）
            var userIdStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized("UserId claim not found");

            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid UserId claim");

            // ✅ 基本防呆（避免資料亂進 DB）
            if (dto.GameId <= 0)
                return BadRequest("Invalid GameId");

            if (dto.Score < 0)
                return BadRequest("Invalid Score");

            await _gameService.SubmitScoreAsync(
                userId,
                dto.Score,
                dto.GameId
            );

            return Ok(new
            {
                success = true,
                userId,
                gameId = dto.GameId,
                score = dto.Score
            });
        }
    }
}

