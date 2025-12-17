namespace PcShop.DTOs
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }          // 主要資料
        public int? TotalItems { get; set; } // 分頁時使用
        public string? Message { get; set; } // 可選，錯誤或提示訊息
    }
}