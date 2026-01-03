namespace PcShop.Areas.Cart.Dtos
{
    public class UserPointDto
    {
        /// <summary>
        /// 使用者目前可用的總點數
        /// </summary>
        public int TotalAvailablePoints { get; set; }

        /// <summary>
        /// 即將到期的點數 (例如 30 天內到期)
        /// </summary>
        public int SoonExpiringPoints { get; set; }
    }
}
