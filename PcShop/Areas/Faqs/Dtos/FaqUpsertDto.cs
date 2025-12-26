namespace PcShop.Areas.Faqs.Dtos
{
    public class FaqUpsertDto
    {
        public long? FaqId { get; set; }
        public int CategoryId { get; set; }
        public string Question { get; set; } = null!;
        public List<FaqBackBlockDto> Blocks { get; set; } = new();
    }
}
