namespace PcShop.Areas.ECPay.Dtos
{
    public class ECPayRequestDto
    {
        public int OrderId { get; set; }
        public int TotalAmount { get; set; }
        public string ItemName { get; set; }
        public string TradeDesc { get; set; }
        public string ChoosePayment { get; set; } = "ALL"; // Credit, ATM, CVS
    }

    public class ECPayResponseDto
    {
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string PaymentType { get; set; } = "aio";
        public int TotalAmount { get; set; }
        public string TradeDesc { get; set; }
        public string ItemName { get; set; }
        public string ReturnURL { get; set; }
        public string ChoosePayment { get; set; }
        public string CheckMacValue { get; set; }
        public string ClientBackURL { get; set; }
        public string OrderResultURL { get; set; }
        public string PaymentInfoURL { get; set; }
        public int EncryptType { get; set; } = 1;
    }
}

