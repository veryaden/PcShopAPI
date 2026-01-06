using Microsoft.Extensions.Configuration;
using PcShop.Areas.ECPay.Dtos;
using PcShop.Areas.ECPay.Repositories;
using PcShop.Models;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PcShop.Areas.ECPay.Services
{
    public class ECPayService : IECPayService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly ExamContext _context;
        private readonly IConfiguration _configuration;

        // 預設測試金鑰 (正式環境建議放 appsettings.json)
        private string MerchantId = "2000132";
        private string HashKey = "5294y06JSMkS4w4G";
        private string HashIV = "v77hoKGq4kWxNNuX";

        public ECPayService(IPaymentRepository paymentRepo, ExamContext context, IConfiguration configuration)
        {
            _paymentRepo = paymentRepo;
            _context = context;
            _configuration = configuration;
        }

        public async Task<Dictionary<string, string>> GetECPayParameters(ECPayRequestDto request)
        {
            // 找尋訂單資訊 (確保訂單存在)
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null) throw new Exception("Order not found");

            // 生成 MerchantTradeNo (必須唯一，通常是 訂單號 + 時間戳)
            string merchantTradeNo = order.OrderNo + DateTime.Now.ToString("HHmmss");

            var parameters = new Dictionary<string, string>
            {
                { "MerchantID", MerchantId },
                { "MerchantTradeNo", merchantTradeNo },
                { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
                { "PaymentType", "aio" },
                { "TotalAmount", request.TotalAmount.ToString() },
                { "TradeDesc", request.TradeDesc ?? "PcShop Payment" },
                { "ItemName", request.ItemName ?? "PcShop Goods" },
                { "ReturnURL", _configuration["FrontendUrl"] + "/api/ECPay/Callback" }, // 綠界伺服器端回傳
                { "ChoosePayment", request.ChoosePayment }, // Credit, ATM, CVS
                { "EncryptType", "1" },
                { "OrderResultURL", _configuration["FrontendUrl"] + "/checkout/success" }, // 付款完成回傳,前端是叫Order-success,要修
                { "ClientBackURL", _configuration["FrontendUrl"] + "/cart" }, // 回到商店按鈕
            };

            // 如果是 ATM 或 CVS，可以設定付款資訊回傳網址
            if (request.ChoosePayment == "ATM" || request.ChoosePayment == "CVS")
            {
                parameters.Add("PaymentInfoURL", _configuration["FrontendUrl"] + "/api/ECPay/Callback");
            }

            // 計算 CheckMacValue
            parameters.Add("CheckMacValue", GetCheckMacValue(parameters));

            // 記錄 Log
            var log = new PaymentLogsEcpay
            {
                OrderId = request.OrderId,
                MerchantTradeNo = merchantTradeNo,
                TradeAmt = request.TotalAmount,
                CheckMacValue = parameters["CheckMacValue"],
                CreateTime = DateTime.Now,
                RtnMsg = "Initial",
                RtnCode = -1
            };
            await _paymentRepo.CreatePaymentLog(log);

            return parameters;
        }

        public async Task<string> ProcessPaymentResult(IFormCollection payInfo)
        {
            // 1. 驗證 CheckMacValue
            var parameters = payInfo.ToDictionary(x => x.Key, x => x.Value.ToString());
            string receivedMac = parameters["CheckMacValue"];
            parameters.Remove("CheckMacValue");

            string calculatedMac = GetCheckMacValue(parameters);
            if (receivedMac != calculatedMac)
            {
                return "0|CheckMacValueVerifyFail";
            }

            // 2. 取得回傳資訊
            string merchantTradeNo = parameters["MerchantTradeNo"];
            string rtnCode = parameters["RtnCode"]; // 1 為成功
            string paymentType = parameters["PaymentType"];

            // 3. 更新 Log 與 訂單狀態
            var log = await _paymentRepo.GetLogByTradeNo(merchantTradeNo);
            if (log != null)
            {
                log.RtnCode = int.Parse(rtnCode);
                log.RtnMsg = parameters["RtnMsg"];
                log.PaymentDate = DateTime.TryParse(parameters["PaymentDate"], out var pDate) ? pDate : (DateTime?)null;
                log.TradeNo = parameters["TradeNo"];
                log.PaymentType = paymentType;
                
                // ATM / CVS 特定參數
                if (parameters.ContainsKey("BankCode")) log.BankCode = parameters["BankCode"];
                if (parameters.ContainsKey("vAccount")) log.VAccount = parameters["vAccount"];
                if (parameters.ContainsKey("PaymentNo")) log.PaymentNo = parameters["PaymentNo"];
                if (parameters.ContainsKey("ExpireDate")) log.ExpireDate = parameters["ExpireDate"];

                await _paymentRepo.UpdatePaymentLog(log);

                // 如果付款成功 (rtnCode == "1")，更新訂單狀態
                if (rtnCode == "1")
                {
                    var order = await _context.Orders.FindAsync(log.OrderId);
                    if (order != null)
                    {
                        order.OrderStatus = 2; // 假設 2 是已付款
                        order.UpdateDate = DateTime.Now;
                        order.SelectedPayment = paymentType;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return "1|OK";
        }

        private string GetCheckMacValue(Dictionary<string, string> parameters) //Dictionary<string, string>這是字典,左邊是key,右邊是Value,可自訂,利用左邊的key找值,一整段為型別
        {
            // 步驟 1：依參數名稱字典排序
            var sortedParams = parameters.OrderBy(p => p.Key)
                .Select(p => $"{p.Key}={p.Value}");

            // 步驟 2：前後加上 HashKey 與 HashIV
            string rawData = $"HashKey={HashKey}&{string.Join("&", sortedParams)}&HashIV={HashIV}";

            // 步驟 3：URL Encode
            string encodedData = HttpUtility.UrlEncode(rawData).ToLower();

            // 綠界特殊的 Encode 補正 (RFC 1738)
            encodedData = encodedData
                .Replace("%2d", "-")
                .Replace("%5f", "_")
                .Replace("%2e", ".")
                .Replace("%21", "!")
                .Replace("%2a", "*")
                .Replace("%28", "(")
                .Replace("%29", ")");

            // 步驟 4：SHA256 加密並轉大寫
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(encodedData);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToUpper();
            }
        }
    }
}

