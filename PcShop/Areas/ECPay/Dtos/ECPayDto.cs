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

    /// <summary>
    /// 綠界超商選站點請求
    /// </summary>
    public class LogisticsMapRequestDto
    {
        public string LogisticsType { get; set; } = "CVS";
        public string LogisticsSubType { get; set; } // FAMI, UNIMART, HILIFE, OKMART
        public string IsCollection { get; set; } = "N"; // 是否代收貨款
        public string? ExtraData { get; set; } // 額外資訊 (例如前端傳回來的暫存ID)
    }
}
