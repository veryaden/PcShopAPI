namespace PcShop.Areas.Ads.Dtos
{
    public class PositionDto
    {
        public int PositionID { get; set; }
        public string Code { get; set; } = null!;
        public string? Name { get; set; }
        public string? Size { get; set; }
        public bool? IsActive { get; set; }
    }
}
