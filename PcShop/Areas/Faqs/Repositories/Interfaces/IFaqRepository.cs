using PcShop.Models;    

namespace PcShop.Areas.Faqs.Repositories.Interfaces
{
    public interface IFaqRepository
    {
        // ===== 前台 =====
        Task<List<Faqcategory>> GetCategoriesAsync();
        Task<List<Faq>> GetFaqsByCategoryAsync(int categoryId);
        Task<Faq?> GetFaqDetailAsync(long faqId);

        // ===== 後台 =====
        Task<List<Faqcategory>> GetAllCategoriesAsync();
        Task AddCategoryAsync(Faqcategory category);

        Task<Faq?> GetByIdAsync(long faqId);
        Task AddAsync(Faq faq);
        Task SaveChangesAsync();
    }

}
