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
            var category = await _service.CreateCategoryAsync(dto);

            return Ok(new FaqCategoryDto
            {
                Id = category.Faqcategoryid,
                CategoryName = category.CategoryName,
                ParentCategoryId = category.ParentCategoryId,
                SortOrder = category.SortOrder
            });
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
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _service.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // 你現在 service 是用 Exception("Category has sub categories") 這種字串判斷
                // 我們先用 409 Conflict 回傳給前端顯示
                return Conflict(new { message = ex.Message });
            }

        }
        [HttpDelete("{faqId:long}")]
        public async Task<IActionResult> DeleteFaq(long faqId)
        {
            await _service.DeleteAsync(faqId);
            return NoContent();
        }
    }

}
