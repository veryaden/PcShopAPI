namespace PcShop.Areas.Games.Strategies.Interfaces
{
    public interface IGamePointCalculator
    {
        /// <summary>
        /// 根據遊戲分數計算回饋點數
        /// </summary>
        int Calculate(int score);
    }
}
