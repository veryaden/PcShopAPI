using PcShop.Areas.Faqs.Dtos;
using PcShop.Models;

namespace PcShop.Areas.Faqs.Services.Interfaces
{
    public interface IFaqService
    {
        // ===== 前台 =====
        Task<List<FaqCategoryDto>> GetCategoriesAsync();

        Task<List<FaqListDto>> GetFaqsAsync(int categoryId);
        Task<FaqDetailDto?> GetFaqDetailAsync(long faqId);

        // ===== 後台 =====
        Task<List<FaqCategoryDto>> GetAllCategoriesForAdminAsync();
        //取得所有分類
        Task<Faqcategory> CreateCategoryAsync(FaqCategoryCreateDto dto);
        //新增 FAQ 分類
        Task<List<FaqListDto>> GetFaqsByCategoryForAdminAsync(int categoryId);
        //依分類取 FAQ 清單
        Task<FaqUpsertDto?> GetFaqForEditAsync(long faqId);
        //取得單筆 FAQ 以供編輯
        Task UpsertAsync(FaqUpsertDto dto);
        //新增或更新 FAQ
        Task DeleteAsync(long faqId);
        //刪除 FAQ
        Task DeleteCategoryAsync(int categoryId);
    }

}
