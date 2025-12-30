using Microsoft.AspNetCore.Mvc;
using PcShop.Ads.Dtos;
using PcShop.Ads.Services.Interfaces;

namespace PcShop.Ads.Controllers;

[ApiController]
[Route("api/admin/ads")]
public class AdminAdsController : ControllerBase
{
    private readonly IAdService _svc;

    public AdminAdsController(IAdService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<IActionResult> List()
        => Ok(await _svc.AdminListAdsAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdUpsertDto dto)
    {
        var id = await _svc.AdminCreateAdAsync(dto);
        return Ok(new { adId = id });
    }

    [HttpPut("{adId:int}")]
    public async Task<IActionResult> Update(int adId, [FromBody] AdUpsertDto dto)
    {
        await _svc.AdminUpdateAdAsync(adId, dto);
        return Ok();
    }

    [HttpDelete("{adId:int}")]
    public async Task<IActionResult> Delete(int adId)
    {
        await _svc.AdminDeleteAdAsync(adId);
        return Ok();
    }

    [HttpGet("report")]
    public async Task<IActionResult> Report([FromQuery] DateTime from, [FromQuery] DateTime to)
        => Ok(await _svc.AdminReportAsync(from, to));
}
