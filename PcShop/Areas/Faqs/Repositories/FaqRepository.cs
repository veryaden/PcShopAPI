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

        public async Task<List<Faqcategory>> GetAllCategoriesAsync()
        {
            return await _context.Faqcategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }
        public async Task AddCategoryAsync(Faqcategory category)
        {
            await _context.Faqcategories.AddAsync(category);
        }

        public async Task<Faq?> GetByIdAsync(long faqId)
        {
            return await _context.Faqs
                .Include(f => f.Faqblocks)
                .FirstOrDefaultAsync(f => f.Faqid == faqId);
        }

        public async Task AddAsync(Faq faq)
        {
            await _context.Faqs.AddAsync(faq);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task RemoveAsync(Faq faq)
        {
            _context.Faqblocks.RemoveRange(faq.Faqblocks);
            _context.Faqs.Remove(faq);
        }
        /// <summary>
        /// 依分類 ID 取得分類（給後台刪除用）
        /// </summary>
        public async Task<Faqcategory?> GetCategoryByIdAsync(int id)
        {
            return await _context.Faqcategories
                .Include(c => c.Faqs) // ⚠️ 判斷能不能刪
                .FirstOrDefaultAsync(c => c.Faqcategoryid == id);
        }

        /// <summary>
        /// 刪除分類（只刪分類本身）
        /// </summary>
        public async Task DeleteCategoryAsync(Faqcategory category)
        {
            _context.Faqcategories.Remove(category);
        }
        public async Task<bool> HasChildCategoriesAsync(int parentCategoryId)
        {
            return await _context.Faqcategories
                .AnyAsync(c => c.ParentCategoryId == parentCategoryId && c.IsActive);
        }
    }

}
