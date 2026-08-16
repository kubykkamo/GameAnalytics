using GameAnalytics.Domain.Services;
using GameAnalytics.Domain.Exceptions;
using GameAnalytics.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GameAnalytics.Application
{
    public class MatchAnalysisService(IRiotApiClient _riotApiService, PlayerStatAnalyser _analyser, ILogger<MatchAnalysisService> _logger)
    {
        public async Task<List<PlayerPerformance>> GetRecentStats(List<string> matches, string gameName, string tagLine)
        {
            var stats = new List<PlayerPerformance>();

            var puuid = await _riotApiService.GetPlayerId(gameName, tagLine);
            matches = matches.Take(10).ToList();

            int count = 0;
            foreach (string match in matches)
            {
                _logger.LogInformation("Fetching match {match}", match);

                var matchStats = await _riotApiService.GetPlayerStats(match, puuid);

                var playerPerformance = _analyser.CalculateMatchStatistics(matchStats);

                stats.Add(playerPerformance);

                count++;

            }

            _logger.LogInformation("Stats created from {count} matches for user: {GameName}#{TagLine}.", count, gameName, tagLine);

            return stats;

        }

        public async Task<List<string>> GetMatchesByAgent(string gameName, string tagLine, string agentName)
        {

            List<string> matches = new List<string>();

            var loweredAgentName = agentName.ToLower();


            string puuid = await _riotApiService.GetPlayerId(gameName, tagLine);

            var matchHistory = await _riotApiService.GetMatches(gameName, tagLine);


            foreach (var match in matchHistory)
            {
                var matchDetails = await _riotApiService.GetMatchDetails(match);

                bool hasAgent = matchDetails.Players.Any(p => p.Puuid == puuid && p.AgentName.ToLower() == loweredAgentName);

                if (hasAgent)
                {
                    matches.Add(matchDetails.MatchId);
                }
            }

            if (!matches.Any()) 
            {
                
                _logger.LogInformation("No matches found with {AgentName}, for {GameName}#{TagLine}", agentName, gameName, tagLine);

                throw new NotFoundException("No matches found with this agent.");
            }
            _logger.LogInformation("Successfully fetched {Count} match with {AgentName}.", matches.Count, agentName);

            return matches;
        }

    }
}