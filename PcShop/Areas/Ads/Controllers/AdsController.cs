using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Ads.Dtos;
using PcShop.Areas.Ads.Services;
using PcShop.Areas.Ads.Services.Interfaces;

namespace PcShop.Areas.Ads.Controllers;

[ApiController]
[Route("api/ads")]
public class AdsController : ControllerBase
{
    private readonly IAdService _service;

    public AdsController(IAdService service)
    {
        _service = service;
    }

    [HttpGet("positions")]
    public async Task<IActionResult> Positions()
        => Ok(await _service.GetPositionsAsync());

    [HttpGet("slot/{positionCode}")]
    public async Task<IActionResult> Slot(string positionCode)
        => Ok(await _service.GetSlotAsync(positionCode));

    [HttpPost("track/click")]
    public async Task<IActionResult> TrackClick([FromBody] TrackClickDto dto)
    {
        if (dto == null || dto.AdId <= 0 || string.IsNullOrWhiteSpace(dto.PositionCode))
            return BadRequest("Invalid payload");

        await _service.TrackClickAsync(dto.AdId, dto.PositionCode.Trim());
        return NoContent();
    }
}
