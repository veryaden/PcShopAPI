using PcShop.Models;    

namespace PcShop.Areas.Faqs.Repositories.Interfaces
{
    public interface IFaqRepository
    {
        Task<List<Faqcategory>> GetCategoriesAsync();
        Task<List<Faq>> GetFaqsByCategoryAsync(int categoryId);
        Task<Faq?> GetFaqDetailAsync(long faqId);
    }

}
