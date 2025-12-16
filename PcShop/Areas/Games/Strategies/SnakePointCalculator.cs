using PcShop.Areas.Games.Strategies.Interfaces;

namespace PcShop.Areas.Games.Strategies
{
    public class SnakePointCalculator : IGamePointCalculator
    {
        public int Calculate(int score)
        {
            // 吃一顆蘋果 1 分，每 5 分換 1 點，至少 1 點
            return Math.Max(1, score / 5);
        }
    }
}
