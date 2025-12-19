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
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductReviews)
            .AsQueryable();

        // 搜尋條件
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.ProductName.Contains(search));

        if (minPrice.HasValue)
            query = query.Where(p => p.BasePrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.BasePrice <= maxPrice.Value);

        if (!string.IsNullOrEmpty(categories))
        {
            var categoryList = categories.Split(',');
            query = query.Where(p => p.Category != null && categoryList.Contains(p.Category.CategoryName));
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
                Category = p.Category != null ? p.Category.CategoryName : "",
                Price = p.BasePrice,
                Rating = p.ProductReviews.Any() ? p.ProductReviews.Average(r => r.Rating) : 0,
                ImageUrl = p.ProductImages.FirstOrDefault() != null ? p.ProductImages.FirstOrDefault().ImageUrl : ""
            })
            .ToListAsync();

        var response = new ApiResponse<List<ProductListDto>>
        {
            Data = items,
            TotalItems = totalCount,
            Message = "成功取得商品列表"
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductDetail(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductReviews)
            .FirstOrDefaultAsync(p => p.ProductId == id && p.Status == 1);

        if (product == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Message = "商品不存在或已下架"
            });
        }

        // 防止 ProductImages 為 null
        var images = product.ProductImages != null
            ? product.ProductImages.Select(i => i.ImageUrl).ToList()
            : new List<string>();

        var dto = new ProductDetailDto
        {
            Id = product.ProductId,
            Name = product.ProductName,
            Category = product.Category != null ? product.Category.CategoryName : "",
            Price = product.BasePrice,
            Description = product.FullDescription ?? "",
            WarrantyInfo = product.WarrantyInfo ?? "",
            Rating = product.ProductReviews.Any() ? product.ProductReviews.Average(r => r.Rating) : 0,
            Images = images
        };

        return Ok(new ApiResponse<ProductDetailDto>
        {
            Data = dto,
            Message = "成功取得商品詳情"
        });
    }
}