using PcShop.Areas.Faqs.Dtos;
using PcShop.Areas.Faqs.Repositories.Interfaces;
using PcShop.Areas.Faqs.Services.Interfaces;

namespace PcShop.Areas.Faqs.Services
{
    public class FaqService : IFaqService
    {
        private readonly IFaqRepository _repo;

        public FaqService(IFaqRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<FaqCategoryDto>> GetCategoriesAsync()
        {
            var categories = await _repo.GetCategoriesAsync();

            return categories.Select(c => new FaqCategoryDto
            {
                Id = c.Faqcategoryid,
                CategoryName = c.CategoryName,
                ParentCategoryId = c.ParentCategoryId,
                SortOrder = c.SortOrder
            }).ToList();
        }

        public async Task<List<FaqListDto>> GetFaqsAsync(int categoryId)
        {
            var faqs = await _repo.GetFaqsByCategoryAsync(categoryId);

            return faqs.Select(f => new FaqListDto
            {
                FAQid = f.Faqid,
                Question = f.Question
            }).ToList();
        }

        public async Task<FaqDetailDto?> GetFaqDetailAsync(long faqId)
        {
            var faq = await _repo.GetFaqDetailAsync(faqId);
            if (faq == null) return null;

            return new FaqDetailDto
            {
                FaqId = faq.Faqid,
                Question = faq.Question,
                Blocks = faq.Faqblocks
                    .OrderBy(b => b.SortOrder)
                    .Select(b => new FaqBlockDto
                    {
                        BlockType = b.BlockType,
                        Content = b.Content,
                        ImageUrl = b.ImageUrl,
                        SortOrder = b.SortOrder
                    }).ToList()
            };
        }
    }

}
