namespace PcShop.Areas.Cart.Dtos
{
    public class CouponValidationDto
    {
        /// <summary>
        /// 驗證是否成功 (true: 可以使用, false: 不能使用)
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 回傳訊息 (例如：「套用成功」或「金額未達門檻」)
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 計算出的折扣金額 (這張券幫使用者省了多少錢)
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 折扣後的最終總金額 (使用者最後要付多少錢)
        /// </summary>
        public decimal FinalTotal { get; set; }
    }
}
