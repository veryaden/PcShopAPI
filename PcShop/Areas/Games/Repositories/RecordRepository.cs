using PcShop.Areas.Games.Repositories.Interfaces;
using PcShop.Models;
using System;

namespace PcShop.Areas.Games.Repositories
{
    public class RecordRepository : IRecordRepository
    {
        private readonly ExamContext _context;

        public RecordRepository(ExamContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Record record)
        {
            _context.Records.Add(record);
            await _context.SaveChangesAsync();
        }
    }

}
