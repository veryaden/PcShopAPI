using Microsoft.AspNetCore.Mvc;
using PcShop.Ads.Dtos;
using PcShop.Ads.Services.Interfaces;

namespace PcShop.Ads.Controllers;

[ApiController]
[Route("api/ads")]
public class AdsController : ControllerBase
{
    private readonly IAdService _svc;

    public AdsController(IAdService svc)
    {
        _svc = svc;
    }

    [HttpGet("positions")]
    public async Task<IActionResult> Positions()
        => Ok(await _svc.GetPositionsAsync());

    [HttpGet("slot/{positionCode}")]
    public async Task<IActionResult> Slot(string positionCode)
        => Ok(await _svc.GetSlotAsync(positionCode));

    [HttpPost("track/click")]
    public async Task<IActionResult> TrackClick([FromBody] TrackClickDto dto)
    {
        await _svc.TrackClickAsync(dto);
        return Ok();
    }
}
