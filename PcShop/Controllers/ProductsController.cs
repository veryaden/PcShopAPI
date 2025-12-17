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
        // 建立查詢，Include 載入相關資料
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
            query = query.Where(p => categoryList.Contains(p.Category.CategoryName));
        }

        // 排序
        query = sort switch
        {
            "priceAsc" => query.OrderBy(p => p.BasePrice),
            "priceDesc" => query.OrderByDescending(p => p.BasePrice),
            _ => query.OrderBy(p => p.ProductName),
        };

        // 計算總筆數，用於分頁
        var totalCount = await query.CountAsync();

        // 取分頁資料並投影成 DTO
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

        // ===============================
        // 主要修改點：使用 ApiResponse<T> 包裝回傳資料
        // ===============================
        var response = new ApiResponse<List<ProductListDto>>
        {
            Data = items,               // 原本的 items 放在 Data
            TotalItems = totalCount,    // 分頁總數
            Message = "成功取得商品列表" // 可選訊息
        };

        // 回傳統一格式
        return Ok(response);
    }
}