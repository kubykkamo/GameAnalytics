using GameAnalytics.Domain.Entities;
namespace GameAnalytics.Domain.Services
{
    public class PlayerStatAnalyser
    {


        public PlayerStatAnalyser() 
        {
            
        }

        public double HeadshotPercentage(PlayerStats playerStats)
        {
            var totalShots = playerStats.Headshots + playerStats.Bodyshots + playerStats.Legshots;
            var headshotPercentage = totalShots > 0 ? (double)playerStats.Headshots / totalShots * 100 : 0;
            return Math.Round(headshotPercentage, 1);
        }

        public double KillToDeathRatio(PlayerStats playerStats)
        {
            return Math.Round(playerStats.Deaths > 0 ? (double)playerStats.Kills / playerStats.Deaths : playerStats.Kills, 2);
        }

        public double KillAssistToDeathRatio(PlayerStats playerStats)
        {
            return Math.Round(playerStats.Deaths > 0 ? (double)(playerStats.Kills + playerStats.Assists) / playerStats.Deaths : (playerStats.Kills + playerStats.Assists), 2);
        }

        public PlayerPerformance CalculateMatchStatistics(PlayerStats playerStats)
        {
            var hs= HeadshotPercentage(playerStats);
            var kd = KillToDeathRatio(playerStats);
            var kda = KillAssistToDeathRatio(playerStats);
            return new PlayerPerformance
            {
                HeadshotPercentage = hs,
                KdRatio = kd,
                KdaRatio = kda,
                Kills = playerStats.Kills,
                Deaths = playerStats.Deaths,
                Assists = playerStats.Assists
            };
        }

    }
}
