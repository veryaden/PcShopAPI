using Microsoft.Extensions.Configuration;
using PcShop.Areas.ECPay.Dtos;
using PcShop.Areas.ECPay.Repositories;
using PcShop.Models;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PcShop.Areas.ECPay.Services
{
    public class ECPayService : IECPayService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IConfiguration _configuration;

        // 綠界公開測試帳號參數
        //private const string MerchantId = "2000132";
        //private const string HashKey = "5294y06JbISpM5x9";
        //private const string HashIV = "v77hoKGq4kWxNNIS";
        //private const string ApiUrl = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5";
        private const string MerchantId = "3002607";
        private const string HashKey = "pwFHCqoQZGmho4w6";
        private const string HashIV = "EkRm7iFT261dpevs";
        private const string ApiUrl = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5";

        public ECPayService(IOrderRepository orderRepo, IPaymentRepository paymentRepo, IConfiguration configuration)
        {
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _configuration = configuration;
        }

        /// <summary>
        /// 產生綠界支付 HTML 表單
        /// </summary>
        public async Task<string> GetECPayParameters(ECPayRequestDto request)
        {
            // 1. 透過 Repository 取得訂單
            var order = await _orderRepo.GetOrderByIdAsync(request.OrderId);
            if (order == null) throw new Exception("Order not found");

            var orderItems = await _orderRepo.GetOrderItemsByOrderIdAsync(request.OrderId);

            // 2. 生成 MerchantTradeNo
            string timestamp = DateTime.Now.ToString("HHmmss");
            string merchantTradeNo = "DMG" + order.OrderId.ToString("D8") + timestamp;
            // 確保不超過 20 碼
            if (merchantTradeNo.Length > 20) merchantTradeNo = merchantTradeNo.Substring(0, 20);

            // 3. 拼接 ItemName (防止過長)
            string itemName = string.Join("#", orderItems.Select(item =>
                $"{item.Sku?.Product?.ProductName ?? "未知商品"} * {item.Quantity}"));

            if (itemName.Length > 200)
            {
                itemName = itemName.Substring(0, 197) + "...";
            }


            //公開URL:https://9rgpr49q-7001.asse.devtunnels.ms

            // 4. 組裝基礎參數
            var dict = new Dictionary<string, string>
            {
                { "MerchantID", MerchantId },
                { "MerchantTradeNo", merchantTradeNo },
                { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
                { "PaymentType", "aio" },
                { "TotalAmount", order.TotalAmount.ToString("0") },
                { "TradeDesc", request.TradeDesc ?? "PcShop_Order_Payment" },
                { "ItemName", string.IsNullOrEmpty(itemName) ? "PcShop_Goods" : itemName },
                { "ReturnURL", _configuration["ECPay:ReturnURL"] ?? "https://9rgpr49q-7001.asse.devtunnels.ms/api/ECPay/Callback" },
                { "ChoosePayment", request.ChoosePayment ?? "ALL" },
                { "EncryptType", "1" },
                { "ClientBackURL", $"{_configuration["FrontendUrl"] ?? "http://localhost:4200"}/home" },
            };

            // 5. 計算 CheckMacValue
            dict.Add("CheckMacValue", CalculateCheckMacValue(dict));

            // 6. 記錄 Log
            var log = new PaymentLogsEcpay
            {
                OrderId = order.OrderId,
                MerchantTradeNo = merchantTradeNo,
                TradeAmt = (int)order.TotalAmount,
                CheckMacValue = dict["CheckMacValue"],
                CreateTime = DateTime.Now,
                RtnMsg = "Initialized",
                RtnCode = -1
            };
            await _paymentRepo.CreatePaymentLog(log);

            // 7. 生成 HTML Form
            return GenerateAutoSubmitForm(dict);
        }

        public async Task<string> ProcessPaymentResult(IFormCollection payInfo)
        {
            var parameters = payInfo.ToDictionary(x => x.Key, x => x.Value.ToString());
            string receivedMac = parameters["CheckMacValue"];
            parameters.Remove("CheckMacValue");

            if (receivedMac != CalculateCheckMacValue(parameters))
            {
                return "0|CheckMacValueVerifyFail";
            }

            string merchantTradeNo = parameters["MerchantTradeNo"];
            string rtnCode = parameters["RtnCode"];

            var log = await _paymentRepo.GetLogByTradeNo(merchantTradeNo);
            if (log != null)
            {
                log.RtnCode = int.Parse(rtnCode);
                log.RtnMsg = parameters["RtnMsg"];
                log.PaymentDate = DateTime.TryParse(parameters["PaymentDate"], out var pDate) ? pDate : (DateTime?)null;
                log.TradeNo = parameters["TradeNo"];
                log.PaymentType = parameters["PaymentType"];

                await _paymentRepo.UpdatePaymentLog(log);

                if (rtnCode == "1")
                {
                    await _orderRepo.UpdateOrderStatusAsync(log.OrderId, 2);
                }
            }

            return "1|OK";
        }

        private string CalculateCheckMacValue(Dictionary<string, string> parameters)
        {
            var sortedParams = parameters.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}");
            string rawData = $"HashKey={HashKey}&{string.Join("&", sortedParams)}&HashIV={HashIV}";

            // URL Encode
            string encoded = HttpUtility.UrlEncode(rawData).ToLower();

            // ⭐ 關鍵修正：.NET 編碼轉換
            encoded = encoded.Replace("%2d", "-")
                             .Replace("%5f", "_")
                             .Replace("%2e", ".")
                             .Replace("%21", "!")
                             .Replace("%2a", "*")
                             .Replace("%28", "(")
                             .Replace("%29", ")");

            // SHA256 加密
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(encoded);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder result = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    result.Append(hash[i].ToString("X2"));
                }
                return result.ToString().ToUpper();
            }
        }

        // ⭐ 修正：這個方法必須在 Class 內部
        private string GenerateAutoSubmitForm(Dictionary<string, string> parameters)
        {
            var html = new StringBuilder();
            html.Append($@"<form id=""ecpay-form"" method=""POST"" action=""{ApiUrl}"">");

            foreach (var kvp in parameters)
            {
                html.Append($@"<input type=""hidden"" name=""{kvp.Key}"" value=""{kvp.Value}"" />");
            }

            html.Append("</form>");
            //html.Append(@"<script type=""text/javascript"">document.getElementById('ecpay-form').submit();</script>");

            return html.ToString();
        }
    }
}