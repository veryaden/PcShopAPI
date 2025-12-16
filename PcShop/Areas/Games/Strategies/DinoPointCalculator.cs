using PcShop.Areas.Games.Strategies.Interfaces;

namespace PcShop.Areas.Games.Strategies
{
    public class DinoPointCalculator : IGamePointCalculator
    {
        public int Calculate(int score)
        {
            // 距離制：跑越遠點數越高
            if (score < 500) return 1;
            if (score > 500) return 2;
            if (score > 2000) return 3;
            if (score > 5000) return 5;
            return 10;
        }
    }
}
