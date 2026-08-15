
namespace GameAnalytics.Domain.Entities
{
    
    public class MatchH2HStats
    {
        public MatchH2HStatsData Data { get; set; }
    }

    public class MatchH2HStatsData
    {
        public H2HKills Kills { get; set; }
    }

    public class H2HKills
    {
    

        public double Round {  get; set; }
    }

   
    

}
