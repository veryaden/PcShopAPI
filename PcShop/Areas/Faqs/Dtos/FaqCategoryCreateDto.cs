namespace PcShop.Areas.Faqs.Dtos
{
    public class FaqCategoryCreateDto
    {
        /// <summary>
        /// 分類名稱
        /// </summary>
        public string CategoryName { get; set; } = null!;

        /// <summary>
        /// 父分類 ID（null = 大分類）
        /// </summary>
        public int? ParentCategoryId { get; set; }

        /// <summary>
        /// 排序（可選）
        /// </summary>
        public int? SortOrder { get; set; }
    }
}