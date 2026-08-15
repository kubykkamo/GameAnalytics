using GameAnalytics.Domain.Entities;
namespace GameAnalytics.Application
{
    

    public interface IRiotApiClient
    {
        Task<string> GetUserId(string gameName, string tagLine);
        Task<AccountInfo> GetAccountInfo(string gameName, string tagLine);
        Task<List<string>> GetMatches(string gameName, string tagLine);
        Task<MatchDetails> GetMatchDetails(string matchId);
        Task<PlayerStats> GetPlayerStats(string matchId, string puuid);
        Task<List<PlayerPerformance>> GetRecentStats(List<string> matches, string puuid);
        Task<List<string>> GetMatchesByAgent(string gameName, string tagLine, string agentName);
    }


}