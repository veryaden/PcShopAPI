namespace PcShop.DTOs
{
    public class ProductSkuDto
    {
        public int Skuid { get; set; }
        public string Skuname { get; set; }
        public int StockQuantity { get; set; }
        public bool IsOutOfStock { get; set; }
        public bool IsOnSale { get; set; }
        public decimal PriceAdjustment { get; set; }
    }
}