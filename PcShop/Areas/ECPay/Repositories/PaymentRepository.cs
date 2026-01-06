using Microsoft.EntityFrameworkCore;
using PcShop.Models;

namespace PcShop.Areas.ECPay.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ExamContext _context;
        public PaymentRepository(ExamContext context) => _context = context;

        public async Task CreatePaymentLog(PaymentLogsEcpay log)
        {
            _context.PaymentLogsEcpays.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<PaymentLogsEcpay> GetLogByTradeNo(string merchantTradeNo)
        {
            return await _context.PaymentLogsEcpays.FirstOrDefaultAsync(x => x.MerchantTradeNo == merchantTradeNo);
        }

        public async Task UpdatePaymentLog(PaymentLogsEcpay log)
        {
            _context.Entry(log).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
