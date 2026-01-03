using PcShop.Areas.ECPay.Dtos;

namespace PcShop.Areas.ECPay.Repositories
{
    public interface IECPayService
    {
        Task<Dictionary<string, string>> GetECPayParameters(ECPayDto request);
        Task<string> ProcessPaymentResult(IFormCollection payInfo);
    }
}
