using PcShop.Domains.Enums;

namespace PcShop.Areas.Games.Dtos
{
    public class SubmitGameScoreDto
    {
        public GameType GameId { get; set; }
        public int Score { get; set; }
    }
}
