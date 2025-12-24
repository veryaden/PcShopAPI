using PcShop.Areas.Faqs.Dtos;
using PcShop.Areas.Faqs.Repositories.Interfaces;
using PcShop.Areas.Faqs.Services.Interfaces;
using PcShop.Models;

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

        public async Task<List<FaqCategoryDto>> GetAllCategoriesForAdminAsync()
        {
            var categories = await _repo.GetAllCategoriesAsync();

            return categories.Select(c => new FaqCategoryDto
            {
                Id = c.Faqcategoryid,
                CategoryName = c.CategoryName,
                ParentCategoryId = c.ParentCategoryId,
                SortOrder = c.SortOrder
            }).ToList();
        }

        public async Task CreateCategoryAsync(FaqCategoryCreateDto dto)
        {
            var entity = new Faqcategory
            {
                CategoryName = dto.CategoryName,
                ParentCategoryId = dto.ParentCategoryId,
                SortOrder = dto.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _repo.AddCategoryAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task<List<FaqListDto>> GetFaqsByCategoryForAdminAsync(int categoryId)
        {
            var faqs = await _repo.GetFaqsByCategoryAsync(categoryId);

            return faqs.Select(f => new FaqListDto
            {
                FAQid = f.Faqid,
                Question = f.Question
            }).ToList();
        }

        public async Task<FaqUpsertDto?> GetFaqForEditAsync(long faqId)
        {
            var faq = await _repo.GetFaqDetailAsync(faqId);
            if (faq == null) return null;

            return new FaqUpsertDto
            {
                FaqId = faq.Faqid,
                CategoryId = faq.CategoryId,
                Question = faq.Question,
                Blocks = faq.Faqblocks
                    .OrderBy(b => b.SortOrder)
                    .Select(b => new FaqBackBlockDto
                    {
                        BlockType = b.BlockType,
                        Content = b.Content,
                        ImageUrl = b.ImageUrl,
                        SortOrder = b.SortOrder
                    }).ToList()
            };
        }



        public async Task UpsertAsync(FaqUpsertDto dto)
        {
            Faq faq;
            var now = DateTime.Now;

            // 1️⃣ 新增 or 編輯 FAQ 主檔
            if (dto.FaqId.HasValue)
            {
                faq = await _repo.GetByIdAsync(dto.FaqId.Value)
                      ?? throw new Exception("FAQ not found");

                faq.Question = dto.Question;
                faq.CategoryId = dto.CategoryId;
                faq.UpdatedAt = now;

                // ⚠️ Answer 不再用，清空或保留都可以（選一）
                faq.Answer = null;

                // 🔥 關鍵：先刪掉舊的 blocks
                faq.Faqblocks.Clear();
            }
            else
            {
                faq = new Faq
                {
                    Question = dto.Question,
                    CategoryId = dto.CategoryId,
                    IsActive = true,
                    CreatedAt = now,
                    Answer = null
                };

                await _repo.AddAsync(faq);
            }

            // 2️⃣ 依 DTO 重建 Faqblock
            foreach (var block in dto.Blocks)
            {
                faq.Faqblocks.Add(new Faqblock
                {
                    BlockType = block.BlockType,
                    Content = block.Content,
                    ImageUrl = block.ImageUrl,
                    SortOrder = block.SortOrder,
                    CreatedAt = now
                });
            }

            // 3️⃣ 儲存
            await _repo.SaveChangesAsync();
        }

    }

}
