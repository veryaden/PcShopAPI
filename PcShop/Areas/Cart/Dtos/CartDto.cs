namespace PcShop.Areas.Cart.Dtos
{
    public class CartDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string spec { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
        public string imageUrl { get; set; }
        public bool selected { get; set; }

    }
}
