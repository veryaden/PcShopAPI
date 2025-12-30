namespace PcShop.Areas.Cart.Dtos
{
    public class PointValidationDto
    {
        /// <summary>
        /// 驗證是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 原始總金額
        /// </summary>
        public decimal OriginalTotal { get; set; }

        /// <summary>
        /// 優惠券折抵金額
        /// </summary>
        public decimal CouponDiscount { get; set; }

        /// <summary>
        /// 點數折抵金額
        /// </summary>
        public int PointDiscount { get; set; }

        /// <summary>
        /// 最終總金額
        /// </summary>
        public decimal FinalTotal { get; set; }
    }
}
