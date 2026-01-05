using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.ECPay.Dtos;
using PcShop.Areas.ECPay.Repositories;

namespace PcShop.Areas.ECPay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ECPayController : ControllerBase
    {
        private readonly IECPayService _ecpayService;

        public ECPayController(IECPayService ecpayService)
        {
            _ecpayService = ecpayService;
        }

        //[HttpPost("GetPaymentParams")]
        //public async Task<IActionResult> GetPaymentParams([FromBody] ECPayDto request)
        //{
        //    var parameters = await _ecpayService.GetECPayParameters(request);
        //    return Ok(parameters);
        //}

        [HttpPost("Callback")]
        public async Task<IActionResult> Callback()
        {
            var result = await _ecpayService.ProcessPaymentResult(Request.Form);
            return Content(result);

        }
    }
}

