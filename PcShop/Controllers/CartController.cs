using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Users.Data;
using PcShop.DTOs;
using PcShop.Models;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ExamContext _context;

    public CartController(ExamContext context)
    {
        _context = context;
    }

    // ---------------------------
    // 加入購物車
    // ---------------------------
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        // 1. 驗證 ModelState
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();

            return BadRequest(new ApiResponse<List<string>>
            {
                Message = "資料驗證失敗",
                Data = errors
            });
        }

        // 2. 查找 SKU
        var sku = await _context.ProductSkus
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Skuid == dto.Skuid);

        if (sku == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Message = "SKU 不存在"
            });
        }

        // 3. 檢查庫存
        if (sku.StockQuantity < dto.Quantity)
        {
            return BadRequest(new ApiResponse<string>
            {
                Message = $"庫存不足，剩餘 {sku.StockQuantity} 件"
            });
        }

        // 4. 取得使用者購物車或建立新的
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == dto.UserId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = dto.UserId,
                CartItems = new List<CartItem>()  // 確保 CartItems 初始化
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        // 5. 保險措施：CartItems 不為 null
        if (cart.CartItems == null)
            cart.CartItems = new List<CartItem>();

        // 6. 合併或新增購物車項目
        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.Skuid == dto.Skuid);
        if (existingItem != null)
        {
            if (existingItem.Quantity + dto.Quantity > sku.StockQuantity)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = $"庫存不足，最多可加入 {sku.StockQuantity - existingItem.Quantity} 件"
                });
            }
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                Skuid = dto.Skuid,
                Quantity = dto.Quantity
            });
        }

        // 7. 儲存更改
        await _context.SaveChangesAsync();

        // 8. 回傳結果
        return Ok(new ApiResponse<string>
        {
            Message = "已成功加入購物車"
        });
    }
}