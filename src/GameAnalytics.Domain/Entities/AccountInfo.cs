namespace GameAnalytics.Domain.Entities
{
    public class AccountInfo
    {
        public required string Puuid { get; set; }
        public int AccountLevel { get; set; }
        public required string Card { get; set; }
    }
}