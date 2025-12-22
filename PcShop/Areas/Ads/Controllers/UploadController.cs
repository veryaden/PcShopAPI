using Microsoft.AspNetCore.Mvc;

namespace PcShop.Areas.Ads.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public UploadController(IWebHostEnvironment env) => _env = env;

        [HttpPost("ad-media")]
        public async Task<IActionResult> UploadAdMedia(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allow = new[] { ".jpg", ".jpeg", ".png", ".webp", ".mp4" };
            if (!allow.Contains(ext)) return BadRequest("Invalid file type");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var folder = Path.Combine(_env.WebRootPath, "uploads", "ads");
            Directory.CreateDirectory(folder);


            var fullPath = Path.Combine(folder, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Ok(new { mediaUrl = $"/uploads/ads/{fileName}" });
        }
    }
}
