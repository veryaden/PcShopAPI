using PcShop.Areas.Faqs.Repositories.Interfaces;
using PcShop.Models;
using Microsoft.EntityFrameworkCore;

namespace PcShop.Areas.Faqs.Repositories
{
    public class FaqRepository : IFaqRepository
    {
        private readonly ExamContext _context;

        public FaqRepository(ExamContext context)
        {
            _context = context;
        }

        public async Task<List<Faqcategory>> GetCategoriesAsync()
        {
            return await _context.Faqcategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Faq>> GetFaqsByCategoryAsync(int categoryId)
        {
            return await _context.Faqs
                .Where(f => f.CategoryId == categoryId && f.IsActive)
                .ToListAsync();
        }

        public async Task<Faq?> GetFaqDetailAsync(long faqId)
        {
            return await _context.Faqs
                .Include(f => f.Faqblocks)
                .FirstOrDefaultAsync(f => f.Faqid == faqId && f.IsActive);
        }
    }

}
