using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Users.Interface;
using PcShop.Models;

namespace PcShop.Areas.Users.Data
{
    public enum OrderStatus 
    {
        Pending = 1,
        Shipping = 2,
        Completed = 3
    }
}

