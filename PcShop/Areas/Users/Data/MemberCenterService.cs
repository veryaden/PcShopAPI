using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using static PcShop.Areas.Users.DTO.MemberCenterDTO;



namespace PcShop.Areas.Users.Data
{
    public class MemberCenterService : IMemberCenterService
    {
        private readonly IMemberCenterData _member;
        private readonly IOrderData _order;

        public MemberCenterService(IMemberCenterData member , IOrderData order)
        {
            _member = member;
            _order = order;
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
                ShipAddress = user.ShippingAddress ?? ""
            };
        }

        public async Task UpdateAddressAsync(int userId, MemberAddressEditDto dto)
        {
            var user = await _member.GetUserForUpdateAsync(userId)
                ?? throw new Exception("使用者不存在");

            user.Address = dto.Address;
            user.ShippingAddress = dto.ShipAddress;
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

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _member.GetUserForUpdateAsync(userId)
                ?? throw new Exception("使用者不存在");

            if ((user.Provider ?? "local") != "local")
                throw new Exception("此帳號為第三方登入，不支援變更密碼");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new Exception("目前密碼錯誤");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdateTime = DateTime.Now;

            await _member.SaveAsync();
        }

        // ✅ 上傳頭像：存到 wwwroot/uploads/avatars
        public async Task<string> UploadAvatarAsync(int userId, IFormFile file)
        {
            var user = await _member.GetUserForUpdateAsync(userId)
                ?? throw new Exception("使用者不存在");

            if (file == null || file.Length == 0) throw new Exception("未選擇檔案");
            if (file.Length > 2 * 1024 * 1024) throw new Exception("檔案太大（上限 2MB）");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext)) throw new Exception("只允許 jpg/jpeg/png/webp");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
            Directory.CreateDirectory(folder);

            var filename = $"{userId}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, filename);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ImageUrl = $"/uploads/avatars/{filename}";
            user.UpdateTime = DateTime.Now;
            await _member.SaveAsync();

            return user.ImageUrl!;
        }


    }

}


