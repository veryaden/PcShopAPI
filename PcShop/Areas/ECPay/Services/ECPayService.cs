using PcShop.Areas.ECPay.Dtos;
using PcShop.Areas.ECPay.Repositories;
using PcShop.Models;

namespace PcShop.Areas.ECPay.Services
{
    public class ECPayService : IECPayService
    {
        private readonly IPaymentRepository _paymentRepo;
        public ECPayService(IPaymentRepository paymentRepo) => _paymentRepo = paymentRepo;

        public async Task<Dictionary<string, string>> GetECPayParameters(ECPayDto request)
        {
            // TODO: 生成 MerchantTradeNo, 存入 Log, 計算 CheckMacValue 等邏輯
            // 您可以將之前的邏輯貼在此處
            return new Dictionary<string, string>(); 
        }

        public async Task<string> ProcessPaymentResult(IFormCollection payInfo)
        {
            // TODO: 處理綠界回傳並更新資料庫
            return "1|OK";
        }
    }
}
