using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcShop.DTOs;
using PcShop.Models;
using System.IdentityModel.Tokens.Jwt;

[Route("api/[controller]")]
[ApiController]
[Authorize] // 需要 JWT 驗證
public class CartController : ControllerBase
{
    private readonly ExamContext _context;

    public CartController(ExamContext context)
    {
        _context = context;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        // 1: Debug - 輸出收到的 JWT
        var token = Request.Headers["Authorization"].ToString();
        Console.WriteLine("Received token: " + token);

        // 2: 驗證 ModelState
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

        // 3: 從 JWT 取得 UserId
        var userIdClaim = User.FindFirst("sub")?.Value
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            Console.WriteLine("Unauthorized: UserId claim missing or invalid");
            return Unauthorized(new ApiResponse<string> { Message = "未授權使用者" });
        }

        Console.WriteLine("UserId from token: " + userId);

        // 4: 查找 SKU
        var sku = await _context.ProductSkus
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Skuid == dto.Skuid);

        if (sku == null)
        {
            return NotFound(new ApiResponse<string> { Message = "SKU 不存在" });
        }

        // 5: 檢查庫存
        if (sku.StockQuantity < dto.Quantity)
        {
            return BadRequest(new ApiResponse<string>
            {
                Message = $"庫存不足，剩餘 {sku.StockQuantity} 件"
            });
        }

        // 6: 取得使用者購物車或建立新的
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CartItems = new List<CartItem>()
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        if (cart.CartItems == null)
            cart.CartItems = new List<CartItem>();

        // 7: 合併或新增購物車項目
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

        // 8: 儲存變更到資料庫
        await _context.SaveChangesAsync();

        // 9: Debug - 確認資料庫內的 CartItems
        var itemsInDb = await _context.CartItems
            .Where(ci => ci.CartId == cart.CartId)
            .ToListAsync();
        Console.WriteLine($"CartItems count for CartId {cart.CartId}: {itemsInDb.Count}");
        foreach (var item in itemsInDb)
        {
            Console.WriteLine($"CartItemID: {item.CartItemId}, Skuid: {item.Skuid}, Quantity: {item.Quantity}");
        }

        // 10: 回傳成功訊息
        return Ok(new ApiResponse<string> { Message = "已成功加入購物車" });
    }
}