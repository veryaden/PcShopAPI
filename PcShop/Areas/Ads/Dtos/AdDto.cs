namespace PcShop.Areas.Ads.Dtos
{
    public class AdDto
    {
        public int AdId { get; set; }
        public string Title { get; set; } = null!;
        public string MediaUrl { get; set; } = null!;
        public string? LinkUrl { get; set; }

        // 位置資訊（方便前端/debug）
        public string PositionCode { get; set; } = null!;
        public string? PositionName { get; set; }
        public string? Size { get; set; }
    }
}
