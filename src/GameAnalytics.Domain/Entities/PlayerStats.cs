namespace GameAnalytics.Domain.Entities
{
    public class PlayerStats
    {
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int Headshots { get; set; }
        public int Bodyshots { get; set; }

        public int Legshots { get; set; }
        public int Score { get; set; }

    }
}