using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcShop.DTOs;
using PcShop.Models; // EF Core Model
using System.IO;

namespace PcShop.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminProductsController : ControllerBase
    {
        private readonly ExamContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminProductsController(ExamContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // -------------------
        // 後台商品列表
        // -------------------
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Select(p => new AdminProductListDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "",
                    BasePrice = p.BasePrice,
                    Status = p.Status,
                    ImageUrl = p.ProductImages
                                .Where(pi => pi.IsMainOrNot == 1)
                                .Select(pi => pi.ImageUrl)
                                .FirstOrDefault() // 找主圖
                                ?? p.ProductImages.Select(pi => pi.ImageUrl).FirstOrDefault() // 找第一張圖
                                ?? "/images/products/noimage.jpg" // 全部沒有就用 noimage
                })
                .ToListAsync();

            return Ok(products);
        }

        // -------------------
        // 取得單一商品
        // -------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            var mainImage = product.ProductImages
                                   .Where(pi => pi.IsMainOrNot == 1)
                                   .Select(pi => pi.ImageUrl)
                                   .FirstOrDefault();

            var dto = new AdminProductDto
            {
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                BasePrice = product.BasePrice,
                Status = product.Status,
                FullDescription = product.FullDescription,
                WarrantyInfo = product.WarrantyInfo,
                ImageUrl = mainImage ?? ""
            };

            return Ok(dto);
        }

        // -------------------
        // 新增商品
        // -------------------
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateProduct([FromBody] AdminProductDto dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                CategoryId = dto.CategoryId,
                BasePrice = dto.BasePrice,
                Status = dto.Status,
                FullDescription = dto.FullDescription,
                WarrantyInfo = dto.WarrantyInfo,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // 單圖
            if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                var image = new ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = dto.ImageUrl,
                    IsMainOrNot = 1
                };
                _context.ProductImages.Add(image);
                await _context.SaveChangesAsync();
            }

            return Ok(new { product.ProductId });
        }

        // -------------------
        // 安全更新商品
        // -------------------
        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] AdminProductDto dto)
        {
            // 1. 找商品
            var product = await _context.Products
                .Include(p => p.ProductImages) // 一併抓圖片
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            // 2. 更新必要欄位（不動 CategoryId）
            product.ProductName = dto.ProductName;
            product.BasePrice = dto.BasePrice;
            product.Status = dto.Status;
            product.FullDescription = dto.FullDescription;
            product.WarrantyInfo = dto.WarrantyInfo;
            product.UpdateTime = DateTime.UtcNow;

            // 3. 更新或新增主圖
            if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                var mainImage = product.ProductImages
                    .FirstOrDefault(pi => pi.IsMainOrNot == 1);

                if (mainImage != null)
                {
                    // 已有主圖 → 更新 URL
                    mainImage.ImageUrl = dto.ImageUrl;
                }
                else
                {
                    // 沒有主圖 → 新增
                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = id,
                        ImageUrl = dto.ImageUrl,
                        IsMainOrNot = 1
                    });
                }
            }

            // 4. 儲存變更
            await _context.SaveChangesAsync();

            return Ok();
        }

        // -------------------
        // 刪除商品
        // -------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            // 刪圖片
            _context.ProductImages.RemoveRange(product.ProductImages);

            // 刪商品
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return Ok();
        }

        // -------------------
        // 上傳圖片（單圖）
        // -------------------
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var savePath = Path.Combine(_env.WebRootPath, "images/products", fileName);

            using var stream = new FileStream(savePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var imageUrl = $"https://localhost:7001/images/products/{fileName}";

            return Ok(new UploadImageResponseDto { ImageUrl = imageUrl });
        }
        // -------------------
        // 取得商品類別列表
        // -------------------
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new { c.CategoryId, c.CategoryName })
                .ToListAsync();

            return Ok(categories);
        }

        // -------------------
        // 新增商品類別
        // -------------------
        [HttpPost("categories")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                return BadRequest("類別名稱不能為空");

            var category = new Category
            {
                CategoryName = dto.CategoryName                
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { category.CategoryId, category.CategoryName });
        }

        // DTO 用來接收新增類別
        public class CategoryDto
        {
            public string CategoryName { get; set; } = "";
        }

        // -------------------
        // 取得某商品的 SKU
        // -------------------
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
                    IsOnSale = s.IsOnSale,
                    PriceAdjustment = s.PriceAdjustment,
                    IsOutOfStock = s.StockQuantity <= 0
                })
                .ToListAsync();

            return Ok(skus);
        }

        // -------------------
        // 新增 SKU
        // -------------------
        [HttpPost("{productId}/skus")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSku(int productId, [FromBody] ProductSkuDto dto)
        {
            var sku = new ProductSku
            {
                ProductId = productId,
                Skuname = dto.Skuname,
                StockQuantity = dto.StockQuantity,
                IsOnSale = dto.IsOnSale,
                PriceAdjustment = dto.PriceAdjustment
            };

            _context.ProductSkus.Add(sku);
            await _context.SaveChangesAsync();

            return Ok(new { sku.Skuid });
        }

        // -------------------
        // 更新 SKU
        // -------------------
        [HttpPut("skus/{skuid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateSku(int skuid, [FromBody] ProductSkuDto dto)
        {
            var sku = await _context.ProductSkus.FindAsync(skuid);
            if (sku == null) return NotFound();

            sku.Skuname = dto.Skuname;
            sku.StockQuantity = dto.StockQuantity;
            sku.IsOnSale = dto.IsOnSale;
            sku.PriceAdjustment = dto.PriceAdjustment;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // -------------------
        // 刪除 SKU
        // -------------------
        [HttpDelete("skus/{skuid}")]
        public async Task<IActionResult> DeleteSku(int skuid)
        {
            var sku = await _context.ProductSkus.FindAsync(skuid);
            if (sku == null) return NotFound();

            _context.ProductSkus.Remove(sku);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}