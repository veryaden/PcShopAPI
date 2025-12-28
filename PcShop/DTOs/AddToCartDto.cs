using System.ComponentModel.DataAnnotations;

namespace PcShop.DTOs
{
    public class AddToCartDto
    {
        public int UserId { get; set; }
        public int Skuid { get; set; }
        public int Quantity { get; set; }
    }
}