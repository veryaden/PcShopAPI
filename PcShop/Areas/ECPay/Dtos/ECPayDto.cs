using System.Collections.Generic;

namespace PcShop.Areas.ECPay.Dtos
{
    /// <summary>
    /// 接收來自前端的結帳請求
    /// </summary>
    public class ECPayRequestDto
    {
        public int OrderId { get; set; }
        public string? TradeDesc { get; set; }
        public string? ChoosePayment { get; set; }
    }

    /// <summary>
    /// 回傳給前端的付款資訊 (包含 HTML 表單)
    /// </summary>
    public class PaymentResultDto
    {
        public string HtmlForm { get; set; }
    }
}
