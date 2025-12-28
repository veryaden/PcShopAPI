using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Ads.Dtos;
using PcShop.Areas.Ads.Services.Interfaces;

namespace PcShop.Areas.Ads.Controllers
{
    [ApiController]
    [Route("api/ads")]
    public class AdsController : ControllerBase
    {
        private readonly IAdService _service;

        public AdsController(IAdService service)
        {
            _service = service;
        }

        // GET /api/ads?positionCode=HOME_CAROUSEL
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string positionCode)
            => Ok(await _service.GetAdsAsync(positionCode));

        // GET /api/ads/positions  (後台下拉選單用)
        [HttpGet("positions")]
        public async Task<IActionResult> Positions()
            => Ok(await _service.GetPositionsAsync());

        // POST /api/ads  (新增/修改)
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] AdUpsertDto dto)
        {
            await _service.UpsertAsync(dto);
            return Ok(new { success = true });
        }

        // POST /api/ads/{adId}/click  (點擊統計)
        [HttpPost("{adId:int}/click")]
        public async Task<IActionResult> Click(int adId)
        {
            await _service.TrackClickAsync(adId);
            return Ok(new { success = true });
        }
    }
}
