using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Ads.Dtos;
using PcShop.Areas.Ads.Services.Interfaces;
using System.Text.Json;

namespace PcShop.Areas.Ads.Controllers;

[ApiController]
[Route("api/admin/ads")]
public class AdminAdsController : ControllerBase
{
    private readonly IAdService _service;
    private readonly IConfiguration _configuration; // ⭐ 新增

    public AdminAdsController(
        IAdService service,
        IConfiguration configuration // ⭐ 新增
    )
    {
        _service = service;
        _configuration = configuration;
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
    public async Task<IActionResult> Report(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
        => Ok(await _service.AdminReportAsync(from, to));

    // =====================================================
    // ⭐ 新增：廣告頁面顯示規則（不動 DB）
    // GET /api/admin/ads/page-rules
    // =====================================================
    [HttpGet("page-rules")]
    public IActionResult GetPageRules()
    {
        var section = _configuration.GetSection("AdPageRules");

        return Ok(new
        {
            Exists = section.Exists(),
            Keys = section.GetChildren().Select(c => c.Key),
            Raw = section.Get<Dictionary<string, string[]>>()
        });
    }
    [HttpPost("page-rules")]
    public IActionResult SavePageRules([FromBody] AdPageRulesDto dto)
    {
        if (dto?.Rules == null)
            return BadRequest();

        var env = HttpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>();

        var filePath = Path.Combine(
            env.ContentRootPath,
            "appsettings.Development.json"
        );

        // 讀現有設定
        var json = System.IO.File.ReadAllText(filePath);
        var root = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;

        // 覆蓋 AdPageRules
        root["AdPageRules"] = dto.Rules;

        // 寫回檔案
        var newJson = JsonSerializer.Serialize(
            root,
            new JsonSerializerOptions { WriteIndented = true }
        );

        System.IO.File.WriteAllText(filePath, newJson);

        return Ok();
    }
}
