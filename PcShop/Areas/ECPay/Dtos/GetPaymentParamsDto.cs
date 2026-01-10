namespace PcShop.Areas.ECPay.Dtos
{
    /// <summary>
    /// 用來接收前端傳來的 OrderId，用於補繳費用或重新付款
    /// </summary>
    public class GetPaymentParamsDto
    {
        public int OrderId { get; set; }
    }
}
