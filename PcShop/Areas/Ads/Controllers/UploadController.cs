using Microsoft.AspNetCore.Mvc;

namespace PcShop.Areas.Ads.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public UploadController(IWebHostEnvironment env) => _env = env;

        // ===== Ads：圖片 + 影片 =====
        [HttpPost("ad-media")]
        [RequestSizeLimit(50_000_000)] // 50MB，可視需求調整
        public async Task<IActionResult> UploadAdMedia([FromForm] IFormFile file)
        {
            var allow = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".mp4", ".webm", ".ogg" };
            return await Upload(file, "ads", allow, returnType: true);
        }

        // ===== FAQ：只允許圖片 =====
        [HttpPost("faq-image")]
        public async Task<IActionResult> UploadFaqImage([FromForm] IFormFile file)
        {
            var allow = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            return await Upload(file, "faqs", allow, returnType: false);
        }

        // ===== 共用邏輯 =====
        private async Task<IActionResult> Upload(IFormFile file, string folderName, string[] allowExts, bool returnType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(ext) || !allowExts.Contains(ext))
                return BadRequest("Invalid file type");

            // 避免 WebRootPath 為 null（少數情況）
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var folder = Path.Combine(webRoot, "uploads", folderName);
            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            var url = $"/uploads/{folderName}/{fileName}";

            // 讓前端好用：同時回傳 url + type
            if (returnType)
            {
                var type = IsVideo(ext) ? "video" : "image";

                // 兼容你原本的 imageUrl 命名（前端若還在用也不會壞）
                return Ok(new
                {
                    url,
                    type,
                    imageUrl = url
                });
            }

            // FAQ 維持你原本回傳格式
            return Ok(new { imageUrl = url });
        }

        private static bool IsVideo(string ext)
        {
            return ext is ".mp4" or ".webm" or ".ogg";
        }
    }
}
