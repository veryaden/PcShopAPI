namespace PcShop.Ads.Dtos;

public class AdUpsertDto
{
    public string Title { get; set; } = "";
    public string MediaUrl { get; set; } = "";
    public string? LinkUrl { get; set; }

    public int PositionId { get; set; }
    public string Type { get; set; } = "image";
    public bool Status { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
