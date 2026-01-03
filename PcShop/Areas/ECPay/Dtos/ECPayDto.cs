namespace PcShop.Areas.ECPay.Dtos
{
    public class ECPayDto
    {
        public class ECPayRequestDto
        {
            public int OrderID { get; set; }           // 關聯你的 Orders 表
            public int TotalAmount { get; set; }        // 訂單總金額
            public string ItemName { get; set; }        // 顯示在金流頁面的商品名稱
            public string TradeDesc { get; set; }       // 交易描述
                                                        // 如果想指定付款方式，可以再加 ChoosePayment 欄位
        }
    }
}
