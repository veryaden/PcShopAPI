using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Users.Data;
using PcShop.DTOs;
using PcShop.Models;
using System.Linq.Dynamic.Core;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ExamContext _context;

    public CategoriesController(ExamContext context)
    {
        _context = context;
    }
    
    // GET: /api/categories
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        //  從資料庫讀取分類
        var categories = await _context.Categories
            .Select(c => new CategoryDto
            {
                Id = c.CategoryId,            // ← 對應資料表
                Name = c.CategoryName
            })
            .ToListAsync();

        //  用 ApiResponse 包起來
        return Ok(new ApiResponse<List<CategoryDto>>
        {
            Data = categories,
            Message = "取得分類成功"
        });
    }
}
