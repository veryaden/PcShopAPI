using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Users.Data; // ExamContext 所在 namespace
using PcShop.DTOs;
using PcShop.Models;
using System.Linq.Dynamic.Core;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ExamContext _context;

    public ProductsController(ExamContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        int page = 1,
        int pageSize = 10,
        string sort = "ProductName",
        string search = "",
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string categories = "")
    {
        // 加上 Include 以載入關聯資料
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductReviews)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.ProductName.Contains(search));

        if (minPrice.HasValue)
            query = query.Where(p => p.BasePrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.BasePrice <= maxPrice.Value);

        if (!string.IsNullOrEmpty(categories))
        {
            var categoryList = categories.Split(',');
            query = query.Where(p => categoryList.Contains(p.Category.CategoryName));
        }

        // 排序
        query = sort switch
        {
            "priceAsc" => query.OrderBy(p => p.BasePrice),
            "priceDesc" => query.OrderByDescending(p => p.BasePrice),
            _ => query.OrderBy(p => p.ProductName),
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListDto
            {
                Id = p.ProductId,
                Name = p.ProductName,
                Category = p.Category.CategoryName,
                Price = p.BasePrice,
                Rating = p.ProductReviews.Any() ? p.ProductReviews.Average(r => r.Rating) : 0,
                ImageUrl = p.ProductImages.FirstOrDefault() != null ? p.ProductImages.FirstOrDefault().ImageUrl : ""
            })
            .ToListAsync();

        return Ok(new { items, totalItems = totalCount });
    }
}