namespace PcShop.Ads.Dtos;

public class AdDto
{
    public int AdId { get; set; }
    public string Title { get; set; } = "";
    public string MediaUrl { get; set; } = "";
    public string? LinkUrl { get; set; }

    public int PositionId { get; set; }
    public string PositionCode { get; set; } = "";  // 方便前端判斷/顯示

    public string Type { get; set; } = "image";     // image | video (varchar(10))
    public bool Status { get; set; }                // bit

    public DateTime? StartTime { get; set; }        // datetime nullable
    public DateTime? EndTime { get; set; }          // datetime nullable
}
