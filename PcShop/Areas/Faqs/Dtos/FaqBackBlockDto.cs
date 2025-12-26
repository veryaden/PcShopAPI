namespace PcShop.Areas.Faqs.Dtos
{
    public class FaqBackBlockDto
    {
        public string BlockType { get; set; } = null!;
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; }
    }

}
