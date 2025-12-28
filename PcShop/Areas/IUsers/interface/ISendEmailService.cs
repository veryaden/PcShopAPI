using PcShop.Areas.Users.DTO;

namespace PcShop.Areas.IUsers.Interface
{
    public interface ISendEmailService
    {
        Task SendAsync(string to, string subject, string html);
    }
}