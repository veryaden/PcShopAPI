namespace PcShop.Areas.Faqs.Dtos
{
    public class FaqCategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? SortOrder { get; set; }
    }

}
