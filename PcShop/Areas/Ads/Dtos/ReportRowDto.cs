namespace PcShop.Areas.Ads.Dtos;

public class ReportRowDto
{
    public DateTime Date { get; set; }          // AdsReport.Date
    public string PositionCode { get; set; } = "";
    public int AdId { get; set; }
    public string Title { get; set; } = "";
    public int Clicks { get; set; }
}
