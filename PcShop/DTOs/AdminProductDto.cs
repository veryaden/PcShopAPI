namespace PcShop.DTOs
{
    public class AdminProductDto
    {
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public decimal BasePrice { get; set; }
        public int Status { get; set; } // 1=上架, 0=下架
        public string FullDescription { get; set; }
        public string WarrantyInfo { get; set; }
        public string ImageUrl { get; set; } // 單圖 URL
    }
}
