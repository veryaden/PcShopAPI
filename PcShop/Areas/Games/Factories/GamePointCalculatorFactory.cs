using PcShop.Areas.Games.Strategies;
using PcShop.Areas.Games.Strategies.Interfaces;
using PcShop.Domains.Enums;

namespace PcShop.Areas.Games.Factories
{
    public class GamePointCalculatorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GamePointCalculatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IGamePointCalculator GetCalculator(GameType gameType)
        {
            return gameType switch
            {
                GameType.Dino => _serviceProvider.GetRequiredService<DinoPointCalculator>(),
                GameType.Snake => _serviceProvider.GetRequiredService<SnakePointCalculator>(),
                _ => throw new Exception($"No calculator")
            };
        }
    }
}
