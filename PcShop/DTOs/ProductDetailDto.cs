namespace PcShop.DTOs;

public class ProductDetailDto
{
	// ===== 基本資訊 =====
	public int Id { get; set; }
	public string Name { get; set; }
	public string Category { get; set; }

	// ===== 價格與狀態 =====
	public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int Status { get; set; }              // 0 下架 / 1 上架
	public DateTime? OnShelfAt { get; set; }

	// ===== 商品描述 =====
	public string Description { get; set; }
	public string WarrantyInfo { get; set; }

	// ===== 圖片 =====
	public List<string> Images { get; set; } = new();

	// ===== SKU / 庫存 =====
	public List<ProductSkuDto> Skus { get; set; } = new();

	// ===== 評價 =====
	public double Rating { get; set; }
	public int ReviewCount { get; set; }
	public List<ProductReviewDto> Reviews { get; set; } = new();
}