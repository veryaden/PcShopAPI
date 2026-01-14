using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using PcShop.Areas.ECPay.Dtos;
using PcShop.Areas.ECPay.Services;

using Microsoft.AspNetCore.Authorization;

namespace PcShop.Areas.ECPay.Controllers
{
    [Area("ECPay")]
    [Route("api/[controller]")]
    [ApiController]
    public class ECPayController : ControllerBase
    {
        private readonly IECPayService _ecpayService;

        public ECPayController(IECPayService ecpayService)
        {
            _ecpayService = ecpayService;
        }

        /// <summary>
        /// 綠界付款回傳結果 (Server-side)
        /// </summary>
        /// <returns></returns>
        [HttpPost("Callback")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Callback([FromForm] IFormCollection collection)
       {
            // 在此處處理綠界回傳的付款結果
            // 比對 CheckMacValue 後更新資料庫訂單狀態
            var result = await _ecpayService.ProcessPaymentResult(collection);
            return Content(result);
        }
         [HttpPost("GetPaymentParams")]
        public async Task<IActionResult> GetPaymentParams([FromBody] GetPaymentParamsDto request)
        {
            try
            {
                // 這裡重複利用你之前寫好的 Service 邏輯
                var ecpayRequest = new ECPayRequestDto
                {
                    OrderId = request.OrderId,
                    TradeDesc = "補繳費用或重新付款"
                };

                // 產生 HTML 表單
                string htmlForm = await _ecpayService.GetECPayParameters(ecpayRequest);

                return Ok(new { success = true, htmlForm });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 前端請求綠界地圖表單
        /// </summary>
        [HttpPost("LogisticsMap")]
        [IgnoreAntiforgeryToken]
        [AllowAnonymous]
        //[Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> LogisticsMap([FromBody] LogisticsMapRequestDto request)
        {
            try
            {
                string htmlForm = await _ecpayService.GetLogisticsMapForm(request);
                return Ok(new { success = true, htmlForm });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 綠界地圖選擇完畢後的回傳 (Server-side)
        /// </summary>
        [HttpPost("LogisticsCallback")]
        [IgnoreAntiforgeryToken]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult LogisticsCallback([FromForm] IFormCollection collection)
        {
            // 綠界選完門市後會由 User 的 Browser POST 過來
            var storeId = collection["CVSStoreID"];
            var storeName = collection["CVSStoreName"];
            var storeAddress = collection["CVSAddress"];
            var extraData = collection["ExtraData"];

            // 建議在 HTML 中加入 <meta charset="utf-8"> 並明確指定 Content-Type 的 charset
            var script = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""utf-8"">
        </head>
        <body>
            <script>
                const storeData = {{
                    storeId: '{storeId}',
                    storeName: '{storeName}',
                    storeAddress: '{storeAddress}',
                    extraData: '{extraData}'
                }};
                // 透過 postMessage 將門市資訊傳回原本的結帳頁面
                if (window.opener) {{
                    window.opener.postMessage(storeData, '*');
                }}
                window.close();
            </script>
        </body>
        </html>";

            // 重點：在第二個參數明確指定 charset=utf-8
            return Content(script, "text/html; charset=utf-8");
        }
    }
}

