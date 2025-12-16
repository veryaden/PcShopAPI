using PcShop.Areas.Games.Factories;
using PcShop.Areas.Games.Repositories.Interfaces;
using PcShop.Domains.Enums;
using PcShop.Models;

namespace PcShop.Areas.Games.Services
{
    public class GameService
    {
        private readonly GamePointCalculatorFactory _calculatorFactory;
        private readonly IRecordRepository _recordRepo;
        private readonly IGamePointRepository _pointRepo;

        public GameService(
            GamePointCalculatorFactory calculatorFactory,
            IRecordRepository recordRepo,
            IGamePointRepository pointRepo)
        {
            _calculatorFactory = calculatorFactory;
            _recordRepo = recordRepo;
            _pointRepo = pointRepo;
        }

        public async Task SubmitScoreAsync(int userId, int score,GameType gameType)
        {
            // 1️⃣ 存原始遊戲紀錄
            await _recordRepo.AddAsync(new Record
            {
                UserId = userId,
                GameId = (int)gameType,
                Score = score,
                PlayedAt = DateTime.Now
            });

            // 2️⃣ 取得算分策略
            var calculator = _calculatorFactory.GetCalculator(gameType);

            int points = calculator.Calculate(score);

            // 3️⃣ 存點數
            await _pointRepo.AddAsync(new GamePoint
            {
                UserId = userId,
                Source = "GAME",
                Points = points,
                ObtainedAt = DateTime.Now,
                Status = 1
            });
        }
    }


}
