using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using PcShop.Areas.ECPay.Dtos;
using PcShop.Areas.ECPay.Repositories;
using Microsoft.AspNetCore.Http;

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
    }

    // 簡單的 DTO，用來接前端傳來的 OrderId
    public class GetPaymentParamsDto
    {
        public int OrderId { get; set; }
    }
}

