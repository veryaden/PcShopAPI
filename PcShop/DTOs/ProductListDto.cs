namespace PcShop.DTOs 
{
	public class ProductListDto 
	{
		public int Id { get; set; }           // ProductId
		public string Name { get; set; }      // ProductName
		public string Category { get; set; }  // CategoryName
		public decimal Price { get; set; }    // BasePrice                                             
        public decimal? SalePrice { get; set; }  // 折扣後價格，可能沒有折扣
        public double Rating { get; set; }    // 平均 Rating
		public string ImageUrl { get; set; }  // 主要 ImageUrl
	}
}