namespace PcShop.Areas.ECPay.Dtos
{
    public class ECPayDto
    {
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string PaymentType { get; set; }
        public int TotalAmount { get; set; }
        public string TradeDesc { get; set; }
        public string ItemName { get; set; }
        public string ReturnURL { get; set; }
        public string OrderResultURL { get; set; }
        public string ChoosePayment { get; set; }
        public int EncryptType { get; set; }
        public string CheckMacValue { get; set; }

        // 額外增加綠界跳轉網址，讓前端更方便
        public string ActionUrl { get; set; } = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5";
    }
    // 2. 如果你希望 Angular 傳入更豐富的資訊 (例如自定義描述)，可以用這個
    public class ECPayRequestDto
    {
        public int OrderId { get; set; }
        public string CustomDescription { get; set; }
    }
}
