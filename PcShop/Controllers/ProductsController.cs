using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Users.Data;
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

    // ---------------------------
    // 取得商品列表
    // ---------------------------
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
            .Where(p => p.Status == 1)
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
            query = query.Where(p => p.Category != null && categoryList.Contains(p.Category.CategoryName));
        }

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

        return Ok(new ApiResponse<List<ProductListDto>>
        {
            Data = items,
            TotalItems = totalCount,
            Message = "成功取得商品列表"
        });
    }

    // ---------------------------
    // 取得商品詳細
    // ---------------------------
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

    // ---------------------------
    // 取得單一商品的 SKU 列表
    // ---------------------------
    [HttpGet("{productId}/skus")]
    public async Task<IActionResult> GetProductSkus(int productId)
    {
        var skus = await _context.ProductSkus
            .Where(s => s.ProductId == productId)
            .Select(s => new ProductSkuDto
            {
                Skuid = s.Skuid,
                Skuname = s.Skuname,
                StockQuantity = s.StockQuantity,
                IsOutOfStock = s.StockQuantity <= 0,
                IsOnSale = s.IsOnSale,
                PriceAdjustment = s.PriceAdjustment
            })
            .ToListAsync();

        if (!skus.Any())
        {
            return NotFound(new ApiResponse<string>
            {
                Message = "此商品沒有 SKU"
            });
        }

        return Ok(new ApiResponse<List<ProductSkuDto>>
        {
            Data = skus,
            Message = "成功取得 SKU 列表"
        });
    }
}