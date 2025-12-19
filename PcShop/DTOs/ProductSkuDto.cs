namespace PcShop.DTOs;

public class ProductSkuDto
{
    public int SkuId { get; set; }
    public string SkuName { get; set; }    // e.g. ¶Â¦â / 512GB
    public int Stock { get; set; }
    public decimal? ExtraPrice { get; set; }
}