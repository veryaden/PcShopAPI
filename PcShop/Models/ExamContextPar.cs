using Microsoft.EntityFrameworkCore;
using PcShop.Models; // 記得換成你的命名空間

namespace PcShop.Models // 命名空間要跟原本的 ExamContext 一樣
{
    // 1. 這裡要加 partial 關鍵字，表示這是 ExamContext 的另一部分
    public partial class ExamContext
    {
        // 2. 實作那個預留的 partial 方法 (注意：不需要 override，也不用 base)
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>(entity =>
            {
                // 設定 Phone 為唯一索引，但排除 NULL 值
                entity.HasIndex(u => u.Phone)
                      .IsUnique()
                      .HasFilter("[Phone] IS NOT NULL");
            });
        }
    }
}