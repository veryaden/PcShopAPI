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
        //取得所有分類
        Task AddCategoryAsync(Faqcategory category);
        //新增 FAQ 分類
        Task<Faq?> GetByIdAsync(long faqId);
        //取得單筆 FAQ
        Task AddAsync(Faq faq);
        //新增 FAQ
        Task SaveChangesAsync();
        //儲存變更
        Task RemoveAsync(Faq faq);
        //刪除 FAQ
        Task<Faqcategory?> GetCategoryByIdAsync(int id);
        Task DeleteCategoryAsync(Faqcategory category);
        Task<bool> HasChildCategoriesAsync(int parentCategoryId);
    }

}
