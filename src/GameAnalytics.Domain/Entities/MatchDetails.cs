namespace GameAnalytics.Domain.Entities
{
    public class MatchDetails
    {
        public required string MatchId { get; set; }
        public required string MapName { get; set; }
        public required string StartedAt { get; set; }
        public required List<MatchPlayer> Players { get; set; }
    }
}