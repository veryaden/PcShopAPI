namespace PcShop.Areas.Ads.Dtos
{
    public class AdUpsertDto
    {
        public int? AdId { get; set; }          // null = 新增; 有值 = 修改
        public string Title { get; set; } = null!;
        public string MediaUrl { get; set; } = null!;
        public string? LinkUrl { get; set; }

        public int? PositionID { get; set; }    // 你 Ads.PositionID 允許 null
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Status { get; set; }
    }
}
