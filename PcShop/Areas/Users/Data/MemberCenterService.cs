using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.Net.Mail;
using static PcShop.Areas.Users.Data.StatusCodeService;
using static PcShop.Areas.Users.DTO.MemberCenterDTO;



namespace PcShop.Areas.Users.Data
{
    public class MemberCenterService : IMemberCenterService
    {
        private readonly IMemberCenterData _member;
        private readonly IOrderData _order;
        private readonly ISendEmailService _email;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public MemberCenterService(IMemberCenterData member , IOrderData order , ISendEmailService email, IWebHostEnvironment env , IConfiguration config)
        {
            _member = member;
            _order = order;
            _email = email;
            _env = env;
            _config = config;
        }
        public async Task<MemberOverviewDto> GetOverviewAsync(int userId)
        {
            var user = await _member.GetUserAsync(userId)
                       ?? throw new Exception("使用者不存在");

            var orders = await _member.GetLatestOrdersAsync(userId, take: 3);

            return new MemberOverviewDto
            {
                Profile = new MemberProfileDto
                {
                    FullName = user.FullName ?? "",
                    BirthDate = user.BirthDay,
                    Phone = user.Phone ?? "",
                    Email = user.Mail ?? ""
                },
                LatestOrders = orders.Select(o =>
                {
                    var status = (OrderStatus)o.OrderStatus;

                    return new MemberLatestOrderDto
                    {
                        OrderNo = o.OrderNo,
                        TotalAmount = o.TotalAmount,
                        StatusText = GetStatusText(status),
                        StatusCode = GetStatusCode(status)
                    };
                }).ToList()
            };
        }

        private static string GetStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "待付款",
                OrderStatus.Shipping => "配送中",
                OrderStatus.Completed => "已完成",
                _ => "處理中"
            };
        }

        private static string GetStatusCode(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "pending",
                OrderStatus.Shipping => "shipping",
                OrderStatus.Completed => "completed",
                _ => "pending"
            };
        }
        public async Task<List<MemberOrderListDto>> GetOrdersAsync(int userId,OrderStatus? status)
        {
            var orders = await _order.GetOrdersAsync(userId, status);

            return orders.Select(o =>
            {
                var s = (OrderStatus)o.OrderStatus;

                return new MemberOrderListDto
                {
                    OrderNo = o.OrderNo,
                    CreateTime = o.CreateDate,
                    TotalAmount = o.TotalAmount,
                    StatusText = GetStatusText(s),
                    StatusCode = GetStatusCode(s)
                };
            }).ToList();
        }


        public async Task<MemberProfileEditDto> GetProfileAsync(int userId)
        {
            var user = await _member.GetUserAsync(userId)
                ?? throw new Exception("使用者不存在");

            return new MemberProfileEditDto
            {
                FullName = user.FullName ?? "",
                BirthDate = user.BirthDay,
                Phone = user.Phone ?? ""
            };
        }

        public async Task UpdateProfileAsync(int userId, MemberProfileEditDto dto)
        {
            var user = await _member.GetUserForUpdateAsync(userId)?? throw new Exception("使用者不存在");

            user.FullName = dto.FullName;
            user.BirthDay = dto.BirthDate;
            user.Phone = dto.Phone;

            await _member.SaveAsync();
        }
        public async Task<MemberAddressEditDto> GetAddressAsync(int userId)
        {
            var user = await _member.GetUserAsync(userId)
                ?? throw new Exception("使用者不存在");

            return new MemberAddressEditDto
            {
                Address = user.Address ?? "",
                ShippingAddress = user.ShippingAddress ?? ""
            };
        }

        public async Task UpdateAddressAsync(int userId, MemberAddressEditDto dto)
        {
            var user = await _member.GetUserForUpdateAsync(userId)
                ?? throw new Exception("使用者不存在");

            user.Address = dto.Address;
            user.ShippingAddress = dto.ShippingAddress;
            user.UpdateTime = DateTime.Now;

            await _member.SaveAsync();
        }

        public async Task<AccountSecurityDto> GetSecurityAsync(int userId)
        {
            var user = await _member.GetUserAsync(userId)
                ?? throw new Exception("使用者不存在");

            return new AccountSecurityDto
            {
                Provider = user.Provider ?? "local"
            };
        }

        public async Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _member.GetUserForUpdateAsync(userId);
            if (user == null) return ServiceResult.Fail("使用者不存在", 404);

            if ((user.Provider ?? "local") != "local")
                return ServiceResult.Fail("此帳號為第三方登入，不支援變更密碼", 403);

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return ServiceResult.Fail("目前密碼錯誤", 400);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdateTime = DateTime.Now;

            await _member.SaveAsync();
            return ServiceResult.Ok();
        }


        public async Task UpdateEmailAsync(int userId, string newEmail, string frontendUrl)
        {
            var user = await _member.GetUserForUpdateAsync(userId)
                ?? throw new Exception("使用者不存在");
            if (user.Provider != "local")
                throw new Exception("第三方登入帳號不可修改 Email");

            if (user.Mail == newEmail)
                throw new Exception("新信箱不可與原信箱相同");

            // 1️⃣ 更新 Email
            user.Mail = newEmail;

            // 2️⃣ 強制重置驗證狀態
            user.IsMailVerified = 0;
            user.EmailVerifyToken = Guid.NewGuid().ToString("N");
            user.EmailVerifyExpireAt = DateTime.Now.AddHours(24);
            user.UpdateTime = DateTime.Now;

            await _member.SaveAsync();

            // 3️⃣ 寄送新驗證信
            var verifyLink = $"{frontendUrl}?token={user.EmailVerifyToken}";

            var html = $@"
        <h2>PCShop 信箱驗證</h2>
        <p>您已修改 Email，請重新完成驗證：</p>
        <a href='{verifyLink}'
           style='display:inline-block;padding:12px 20px;
                  background:#2a7bff;color:#fff;
                  text-decoration:none;border-radius:6px;'>
           重新驗證 Email
        </a>
        <p>此連結 24 小時內有效</p>";
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new Exception("新 Email 不可為空");

            if (!MailAddress.TryCreate(newEmail, out _))
                throw new Exception("Email 格式不正確");

            PrepareEmailVerification(user);
            await _email.SendAsync(newEmail, "PCShop 重新驗證 Email", html);
        }


        // ✅ 上傳頭像：存到 wwwroot/uploads/avatars
        public async Task<string> UploadAvatarAsync(int userId, IFormFile file)
        {
            var user = await _member.GetUserForUpdateAsync(userId)
                ?? throw new Exception("使用者不存在");

            if (file == null || file.Length == 0)
                throw new Exception("未選擇檔案");

            if (file.Length > 2 * 1024 * 1024)
                throw new Exception("檔案太大（上限 2MB）");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext))
                throw new Exception("只允許 jpg / jpeg / png / webp");

            // ✅ 正確的 WebRoot 路徑
            var folder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(folder);

            var filename = $"{userId}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, filename);

            // 1️⃣ 存新檔
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 2️⃣ 刪舊檔（如果有）
            if (!string.IsNullOrWhiteSpace(user.ImageUrl))
            {
                var oldPath = Path.Combine(
                    _env.WebRootPath,
                    user.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            // 3️⃣ 更新 DB
            user.ImageUrl = $"/uploads/avatars/{filename}";
            user.UpdateTime = DateTime.Now;

            await _member.SaveAsync();

            return user.ImageUrl;
        }


        public async Task SendVerifyEmailAsync(int userId, string frontendUrl)
        {
            var user = await _member.GetUserAsync(userId)
                ?? throw new Exception("使用者不存在");

            if (user.IsMailVerified == 1)
                throw new Exception("信箱已驗證");

            user.EmailVerifyToken = Guid.NewGuid().ToString("N");
            user.EmailVerifyExpireAt = DateTime.Now.AddHours(24);

            await _member.SaveAsync();

            var verifyLink = $"{frontendUrl}?token={user.EmailVerifyToken}";

            var html = $@"
        <h2>PCShop 信箱驗證</h2>
        <p>請點擊下方按鈕完成驗證：</p>
        <a href='{verifyLink}'
           style='display:inline-block;padding:12px 20px;
                  background:#2a7bff;color:#fff;
                  text-decoration:none;border-radius:6px;'>
           驗證我的 Email
        </a>
        <p>此連結 24 小時內有效</p>";
            var mailToSend = user.Mail; // 🔒 快照
            if (string.IsNullOrWhiteSpace(mailToSend))
                throw new Exception("Email 為空");


            PrepareEmailVerification(user);
            await _email.SendAsync(mailToSend, "PCShop 信箱驗證", html);
        }

        public async Task ConfirmEmailAsync(string token)
        {
            var user = await _member.GetUserByEmailTokenAsync(token)
                ?? throw new Exception("驗證連結無效");

            if (user.EmailVerifyExpireAt < DateTime.Now)
                throw new Exception("驗證連結已過期");

            user.IsMailVerified = 1;
            user.EmailVerifyToken = null;
            user.EmailVerifyExpireAt = null;
            user.UpdateTime = DateTime.Now;

            await _member.SaveAsync();
        }
        private void PrepareEmailVerification(UserProfile user)
        {
            user.IsMailVerified = 0;
            user.EmailVerifyToken = Guid.NewGuid().ToString("N");
            user.EmailVerifyExpireAt = DateTime.Now.AddHours(24);
            user.UpdateTime = DateTime.Now;
        }
    }

}


