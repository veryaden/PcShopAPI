using PcShop.Models;

namespace PcShop.Areas.Games.Repositories.Interfaces
{
    public interface IRecordRepository
    {
        Task AddAsync(Record record);
    }
}
