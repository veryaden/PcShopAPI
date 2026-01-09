using PcShop.Models;
using System.Threading.Tasks;

namespace PcShop.Areas.ECPay.Repositories
{
    public interface IPaymentRepository
    {
        Task CreatePaymentLog(PaymentLogsEcpay log);
        Task<PaymentLogsEcpay> GetLogByTradeNo(string merchantTradeNo);
        Task UpdatePaymentLog(PaymentLogsEcpay log);
    }
}
