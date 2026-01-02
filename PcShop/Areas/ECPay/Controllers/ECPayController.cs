//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using PcShop.Models;

//namespace PcShop.Areas.ECPay.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ECPayController : ControllerBase
//    {
//            private readonly ILogger<ECPayController> _logger;
//            private readonly ExamContext _context;

//            public ECPayController(ILogger<ECPayController> logger, ECPayContext context)
//            {
//                _context = context;
//                _logger = logger;
//            }

//            // STEP 1: Angular 呼叫此 API 建立訂單並取得金流參數
//            [HttpPost("CreateOrder")]
//            public async Task<ActionResult<Dictionary<string, string>>> CreateOrder([FromBody] ECPayDTO ecPayDto)
//            {
//                // 補全必要的金流欄位
//                ecPayDto.MerchantID = "3002607"; // 測試商號
//                ecPayDto.MerchantTradeNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
//                ecPayDto.MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
//                ecPayDto.PaymentType = "aio";

//                // 1. 存入資料庫
//                EcpayOrders order = new EcpayOrders
//                {
//                    MemberId = ecPayDto.MerchantID,
//                    MerchantTradeNo = ecPayDto.MerchantTradeNo,
//                    RtnCode = 0, // 未付款
//                    RtnMsg = "訂單成功尚未付款",
//                    TradeNo = ecPayDto.MerchantID,
//                    TradeAmt = ecPayDto.TotalAmount,
//                    PaymentDate = Convert.ToDateTime(ecPayDto.MerchantTradeDate),
//                    PaymentType = ecPayDto.PaymentType,
//                    PaymentTypeChargeFee = "0",
//                    TradeDate = ecPayDto.MerchantTradeDate,
//                    SimulatePaid = 0
//                };
//                _context.EcpayOrders.Add(order);
//                await _context.SaveChangesAsync();

//                // 2. 準備傳送給綠界的參數
//                var website = $"https://localhost:7001"; // 你的後端網址
//                var angularApp = $"http://localhost:4200"; // 你的前端網址

//                var ecpayParams = new Dictionary<string, string>
//            {
//                { "MerchantID", ecPayDto.MerchantID },
//                { "MerchantTradeNo",  ecPayDto.MerchantTradeNo},
//                { "MerchantTradeDate",  ecPayDto.MerchantTradeDate},
//                { "PaymentType",  ecPayDto.PaymentType},
//                { "TotalAmount",  ecPayDto.TotalAmount.ToString()},
//                { "TradeDesc", ecPayDto.TradeDesc ?? "無" },
//                { "ItemName",  ecPayDto.ItemName ?? "商品"},
//                { "ReturnURL",  $"{website}/api/ECPay/AddPayInfo"}, // Server 端回傳
//                { "OrderResultURL", $"{website}/api/ECPay/PaymentResult"}, // 頁面跳轉回傳
//                { "ChoosePayment", "ALL"},
//                { "EncryptType",  "1"},
//            };

//                // 3. 計算檢查碼
//                ecpayParams["CheckMacValue"] = GetCheckMacValue(ecpayParams);

//                return Ok(ecpayParams);
//            }

//            // STEP 2: 處理綠界伺服器端通知 (ReturnURL)
//            [HttpPost("AddPayInfo")]
//            public async Task<string> AddPayInfo([FromForm] IFormCollection payInfo)
//            {
//                _logger.LogInformation($"收到綠界付款通知: {payInfo["MerchantTradeNo"]}, RtnCode: {payInfo["RtnCode"]}");

//                var merchantTradeNo = payInfo["MerchantTradeNo"];
//                var order = _context.EcpayOrders.FirstOrDefault(m => m.MerchantTradeNo == merchantTradeNo);

//                if (order != null)
//                {
//                    order.RtnCode = int.Parse(payInfo["RtnCode"]);
//                    order.RtnMsg = payInfo["RtnMsg"];
//                    if (payInfo["RtnCode"] == "1") // 1 為付款成功
//                    {
//                        order.PaymentDate = Convert.ToDateTime(payInfo["PaymentDate"]);
//                        order.TradeNo = payInfo["TradeNo"];
//                    }
//                    await _context.SaveChangesAsync();
//                }

//                return "1|OK"; // 綠界要求回傳 1|OK
//            }

//            // STEP 3: 處理頁面跳轉回傳 (OrderResultURL)
//            // 綠界會 POST 到這裡，我們更新狀態後 redirect 回 Angular
//            [HttpPost("PaymentResult")]
//            public async Task<IActionResult> PaymentResult([FromForm] IFormCollection payInfo)
//            {
//                var merchantTradeNo = payInfo["MerchantTradeNo"];
//                _logger.LogInformation($"用戶付款完成跳轉: {merchantTradeNo}");

//                // 更新訂單狀態 (可選，通常 ReturnURL 就更新過了)
//                var order = _context.EcpayOrders.FirstOrDefault(m => m.MerchantTradeNo == merchantTradeNo);
//                if (order != null && order.RtnCode != 1)
//                {
//                    order.RtnCode = int.Parse(payInfo["RtnCode"]);
//                    order.RtnMsg = payInfo["RtnMsg"];
//                    await _context.SaveChangesAsync();
//                }

//                // 導向 Angular 的結果頁面
//                var angularApp = "http://localhost:4200";
//                return Redirect($"{angularApp}/payment/result?no={merchantTradeNo}&status={payInfo["RtnCode"]}");
//            }

//            #region Encryption Logic (Moved from HomeController)
//            private string GetCheckMacValue(Dictionary<string, string> order)
//            {
//                var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
//                string checkValue = string.Join("&", param);
//                var hashKey = "pwFHCqoQZGmho4w6"; // 測試用
//                var hashIV = "EkRm7iFT261dpevs";  // 測試用
//                checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={hashIV}";
//                checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
//                checkValue = GetSHA256(checkValue);
//                return checkValue.ToUpper();
//            }

//            private string GetSHA256(string value)
//            {
//                var result = new StringBuilder();
//                var sha256 = SHA256.Create();
//                var bts = Encoding.UTF8.GetBytes(value);
//                var hash = sha256.ComputeHash(bts);
//                for (int i = 0; i < hash.Length; i++)
//                {
//                    result.Append(hash[i].ToString("X2"));
//                }
//                return result.ToString();
//            }
//            #endregion
//        }
//    }

