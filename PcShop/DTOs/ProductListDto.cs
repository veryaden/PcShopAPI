namespace PcShop.DTOs 
{
	public class ProductListDto 
	{
		public int Id { get; set; }           // ProductId
		public string Name { get; set; }      // ProductName
		public string Category { get; set; }  // CategoryName
		public decimal Price { get; set; }    // BasePrice
		public double Rating { get; set; }    // ¥­§¡ Rating
		public string ImageUrl { get; set; }  // ¥D­n ImageUrl
	}
}