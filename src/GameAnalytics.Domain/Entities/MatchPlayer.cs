namespace GameAnalytics.Domain.Entities
{
    public class MatchPlayer
    {
        public required string Puuid { get; set; }
        public required string Name { get; set; }
        public required string Tag { get; set; }
        public required string TeamId { get; set; }
        public required string AgentName { get; set; }
        public required PlayerStats Stats { get; set; }
    }
}