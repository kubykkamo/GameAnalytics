using GameAnalytics.Domain.Entities;
namespace GameAnalytics.Application
{
    

    public interface IRiotApiClient
    {
        Task<string> GetPlayerId(string gameName, string tagLine);
        Task<AccountInfo> GetAccountInfo(string gameName, string tagLine);
        Task<List<string>> GetMatches(string gameName, string tagLine);
        Task<MatchDetails> GetMatchDetails(string matchId);
        Task<PlayerStats> GetPlayerStats(string matchId, string puuid);
        
    }


}