using PcShop.Areas.Faqs.Dtos;

namespace PcShop.Areas.Faqs.Services.Interfaces
{
    public interface IFaqService
    {
        Task<List<FaqCategoryDto>> GetCategoriesAsync();
        Task<List<FaqListDto>> GetFaqsAsync(int categoryId);
        Task<FaqDetailDto?> GetFaqDetailAsync(long faqId);
    }

}
