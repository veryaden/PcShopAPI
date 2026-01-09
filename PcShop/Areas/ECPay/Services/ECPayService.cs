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

        // 綠界公開測試帳號參數 (從 RTF 範本確認)
        private const string MerchantId = "2000132";
        private const string HashKey = "5294y06JbISpM5x9";
        private const string HashIV = "v77hoKGq4kWxNNIS";
        private const string ApiUrl = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5"; //金流網址

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
            // 1. 透過 Repository 取得訂單與明細 (DAL 層分離)
            var order = await _orderRepo.GetOrderByIdAsync(request.OrderId);
            if (order == null) throw new Exception("Order not found");

            var orderItems = await _orderRepo.GetOrderItemsByOrderIdAsync(request.OrderId);

            // 2. 生成 MerchantTradeNo (參考 RTF 邏輯: FZ + 8位ID + 6位時間)
            // 格式: FZ + OrderID(補滿8位) + 時分秒
            string timestamp = DateTime.Now.ToString("HHmmss");
            string merchantTradeNo = "FZ" + order.OrderId.ToString("D8") + timestamp;
            if (merchantTradeNo.Length > 20) merchantTradeNo = merchantTradeNo.Substring(0, 20);

            // 3. 拼接 ItemName (讀取真實商品名稱)
            string itemName = string.Join("#", orderItems.Select(item => 
                $"{item.Sku?.Product?.ProductName ?? "未知商品"} * {item.Quantity}"));

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
                { "ReturnURL", _configuration["FrontendUrl"] ?? "https://localhost:7007" + "/api/ECPay/Callback" }, 
                { "ChoosePayment", request.ChoosePayment ?? "ALL" }, 
                { "EncryptType", "1" },
                { "ClientBackURL", "http://localhost:4200/cart" }, // 完成支付後的「返回特店」按鈕
            };

            // 5. 計算 CheckMacValue 並加入
            dict.Add("CheckMacValue", CalculateCheckMacValue(dict));

            // 6. 記錄 Payment Log (DAL 層)
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

            // 7. 生成自動提交的 HTML Form
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

                // 如果付款成功 (RtnCode == 1)，則更新訂單狀態
                if (rtnCode == "1")
                {
                    await _orderRepo.UpdateOrderStatusAsync(log.OrderId, 1); // 假設 1 為已付款/待出貨
                }
            }

            return "1|OK";
        }

        private string CalculateCheckMacValue(Dictionary<string, string> parameters)
        {
            var sortedParams = parameters.OrderBy(p => p.Key)
                .Select(p => $"{p.Key}={p.Value}");

            string rawData = $"HashKey={HashKey}&{string.Join("&", sortedParams)}&HashIV={HashIV}";

            string encodedData = HttpUtility.UrlEncode(rawData).ToLower();

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(encodedData);
                byte[] hash = sha256.ComputeHash(bytes);
                
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    result.Append(hash[i].ToString("X2"));
                }
                return result.ToString().ToUpper();
            }
        }

        private string GenerateAutoSubmitForm(Dictionary<string, string> parameters)
        {
            var html = new StringBuilder();
            html.Append($@"<form id=""ecpay-form"" method=""POST"" action=""{ApiUrl}"">");

            foreach (var kvp in parameters)
            {
                html.Append($@"<input type=""hidden"" name=""{kvp.Key}"" value=""{kvp.Value}"" />");
            }

            html.Append("</form>");
            html.Append(@"<script type=""text/javascript"">document.getElementById('ecpay-form').submit();</script>");

            return html.ToString();
        }
    }
}
