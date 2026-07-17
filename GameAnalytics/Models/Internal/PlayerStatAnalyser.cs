using GameAnalytics.Services;
using GameAnalytics.Models.External;

namespace GameAnalytics.Models.Internal
{
    public class PlayerStatAnalyser
    {


        public PlayerStatAnalyser() 
        {
            
        }

        public double HeadshotPercentage(PlayerStatsDto playerStats)
        {
            var totalShots = playerStats.Headshots + playerStats.Bodyshots + playerStats.Legshots;
            var headshotPercentage = totalShots > 0 ? (double)playerStats.Headshots / totalShots * 100 : 0;
            return Math.Round(headshotPercentage, 1);
        }

        public double KillToDeathRatio(PlayerStatsDto playerStats)
        {
            return Math.Round(playerStats.Deaths > 0 ? (double)playerStats.Kills / playerStats.Deaths : playerStats.Kills, 2);
        }

        public double KillAssistToDeathRatio(PlayerStatsDto playerStats)
        {
            return Math.Round(playerStats.Deaths > 0 ? (double)(playerStats.Kills + playerStats.Assists) / playerStats.Deaths : (playerStats.Kills + playerStats.Assists), 2);
        }

        public PlayerPerformanceDto CalculateMatchStatistics(PlayerStatsDto playerStats)
        {
            var hs= HeadshotPercentage(playerStats);
            var kd = KillToDeathRatio(playerStats);
            var kda = KillAssistToDeathRatio(playerStats);
            return new PlayerPerformanceDto
            {
                headshotPercentage = hs,
                kdRatio = kd,
                kdaRatio = kda,
            };
        }

    }
}
