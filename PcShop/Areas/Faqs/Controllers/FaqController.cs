using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Faqs.Services.Interfaces;

[ApiController]
[Route("api/faq")]
public class FaqController : ControllerBase
{
    private readonly IFaqService _service;

    public FaqController(IFaqService service)
    {
        _service = service;
    }

    // ① 分類
    [HttpGet("categories")]
    public async Task<IActionResult> Categories()
        => Ok(await _service.GetCategoriesAsync());

    // ② FAQ 清單（第三層）
    [HttpGet("by-category/{categoryId:int}")]
    public async Task<IActionResult> FaqsByCategory(int categoryId)
        => Ok(await _service.GetFaqsAsync(categoryId));

    // ③ FAQ 詳細（第四層）
    [HttpGet("{faqId:long}")]
    public async Task<IActionResult> Detail(long faqId)
    {
        var result = await _service.GetFaqDetailAsync(faqId);
        return result == null ? NotFound() : Ok(result);
    }
}
