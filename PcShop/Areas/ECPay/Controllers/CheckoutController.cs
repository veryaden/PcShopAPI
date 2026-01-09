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
    }
}
