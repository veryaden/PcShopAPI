namespace PcShop.DTOs;

public class ProductReviewDto
{
    public string UserName { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}