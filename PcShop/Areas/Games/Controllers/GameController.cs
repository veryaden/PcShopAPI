using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Games.Dtos;
using PcShop.Areas.Games.Services;
using PcShop.Domains.Enums;

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

        [Authorize] // ⭐ 先加上
        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore(
            [FromBody] SubmitGameScoreDto dto)
        {
            // ⚠️ 暫時假資料（之後 JWT）
            var userIdClaim = User.FindFirst("userId");

            /*int userId = userIdClaim != null
                ? int.Parse(userIdClaim.Value)
                : 1; // ⚠️ JWT 尚未接好前的 fallback
              */         
            int userId = 1;
            await _gameService.SubmitScoreAsync(
                userId,
                dto.Score,
                dto.GameId
            );

            return Ok(new { success = true });
        }

    }
}

