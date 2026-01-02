using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Ads.Dtos;
using PcShop.Areas.Ads.Services.Interfaces;

namespace PcShop.Areas.Ads.Controllers;

[ApiController]
[Route("api/admin/ads")]
public class AdminAdsController : ControllerBase
{
    private readonly IAdService _service;

    public AdminAdsController(IAdService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> List()
        => Ok(await _service.AdminListAdsAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdUpsertDto dto)
    {
        var id = await _service.AdminCreateAdAsync(dto);
        return Ok(new { adId = id });
    }

    [HttpPut("{adId:int}")]
    public async Task<IActionResult> Update(int adId, [FromBody] AdUpsertDto dto)
    {
        await _service.AdminUpdateAdAsync(adId, dto);
        return Ok();
    }

    [HttpDelete("{adId:int}")]
    public async Task<IActionResult> Delete(int adId)
    {
        await _service.AdminDeleteAdAsync(adId);
        return Ok();
    }

    [HttpGet("report")]
    public async Task<IActionResult> Report([FromQuery] DateTime from, [FromQuery] DateTime to)
        => Ok(await _service.AdminReportAsync(from, to));
}
