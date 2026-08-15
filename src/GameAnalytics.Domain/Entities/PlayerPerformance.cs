namespace GameAnalytics.Domain.Entities
{
    public class PlayerPerformance
    {
        public double HeadshotPercentage { get; set; }
        public double KdRatio { get; set; }
        public double KdaRatio { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
    }
}