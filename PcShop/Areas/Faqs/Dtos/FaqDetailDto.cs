using PcShop.Areas.Faqs.Dtos;

public class FaqDetailDto
{
    public long FaqId { get; set; }
    public string Question { get; set; }
    public List<FaqBlockDto> Blocks { get; set; }
}

