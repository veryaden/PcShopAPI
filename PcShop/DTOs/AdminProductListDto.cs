namespace PcShop.DTOs
{
    public class AdminProductListDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }   // <-- 新增這行
        public string CategoryName { get; set; }
        public decimal BasePrice { get; set; }
        public int Status { get; set; }
        public string ImageUrl { get; set; } // 主圖 URL
    }
}
