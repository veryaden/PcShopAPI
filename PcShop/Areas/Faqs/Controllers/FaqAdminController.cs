using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Faqs.Dtos;
using PcShop.Areas.Faqs.Services.Interfaces;

namespace PcShop.Areas.Faqs.Controllers
{
    [ApiController]
    [Route("api/faqs-admin")]
    public class FaqAdminController : ControllerBase
    {
        private readonly IFaqService _service;

        public FaqAdminController(IFaqService service)
        {
            _service = service;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
            => Ok(await _service.GetAllCategoriesForAdminAsync());

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] FaqCategoryCreateDto dto)
        {
            await _service.CreateCategoryAsync(dto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFaqs([FromQuery] int categoryId)
            => Ok(await _service.GetFaqsByCategoryForAdminAsync(categoryId));

        [HttpGet("{faqId}")]
        public async Task<IActionResult> GetForEdit(long faqId)
            => Ok(await _service.GetFaqForEditAsync(faqId));

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert([FromBody] FaqUpsertDto dto)
        {
            await _service.UpsertAsync(dto);
            return Ok();
        }
    }

}
