using PcShop.Areas.ECPay.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PcShop.Areas.ECPay.Services
{
     public interface IECPayService
    {
        Task<string> GetECPayParameters(ECPayRequestDto request);
        Task<string> ProcessPaymentResult(IFormCollection payInfo);
        Task<string> GetLogisticsMapForm(LogisticsMapRequestDto request);
    }
}
