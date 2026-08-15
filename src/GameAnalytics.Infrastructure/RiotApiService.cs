using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GameAnalytics.Domain.Services;
using GameAnalytics.Domain.Entities;
using GameAnalytics.Domain.Exceptions;
using GameAnalytics.Application;
namespace GameAnalytics.Infrastructure
{
    public class RiotApiService(
    HttpClient _httpClient, 
    IConfiguration _configuration, 
    PlayerStatAnalyser _analyser, 
    ILogger<RiotApiService> _logger) : IRiotApiClient
    {
        

        

        public async Task<string> GetUserId(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);
            var url = $"https://api.henrikdev.xyz/valorant/v2/account/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);

            JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("data").GetProperty("puuid").GetString();


        }
        public async Task<List<string>> GetMatches(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);

            var url = $"https://api.henrikdev.xyz/valorant/v4/matches/eu/pc/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);

            var jsonString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var matchList = JsonSerializer.Deserialize<MatchListDto>(jsonString, options);

            var readyList = matchList.Data.Select(x => x.MetaData.Id).ToList();

           

            return readyList;
        }

        public async Task<MatchDetails> GetMatchDetails(string matchId)
        {
            var safeMatchId = Uri.EscapeDataString(matchId);
            var url = $"https://api.henrikdev.xyz/valorant/v4/match/eu/{safeMatchId}";
            var response = await _httpClient.GetAsync(url);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonString = await response.Content.ReadAsStringAsync();
            var raw = JsonSerializer.Deserialize<SingleMatchResponseDto>(jsonString, options);

             return new MatchDetails
            {
                MatchId = raw.Data.Metadata.MatchId,
                MapName = raw.Data.Metadata.Map?.Name ?? "",
                StartedAt = raw.Data.Metadata.StartedAt,
                Players = raw.Data.Players.Select(p => new MatchPlayer
                {
                    Puuid = p.Puuid,
                    Name = p.Name,
                    Tag = p.Tag,
                    TeamId = p.TeamId,
                    AgentName = p.Agent?.Name ?? "",
                    Stats = new PlayerStats
                    {
                        Kills = p.Stats.Kills,
                        Deaths = p.Stats.Deaths,
                        Assists = p.Stats.Assists,
                        Headshots = p.Stats.Headshots,
                        Bodyshots = p.Stats.Bodyshots,
                        Legshots = p.Stats.Legshots
                    }
                }
                ).ToList()

            };
        }

        public async Task<PlayerStats> GetPlayerStats(string matchId, string puuid)
        {
            ArgumentException.ThrowIfNullOrEmpty(matchId);
            ArgumentException.ThrowIfNullOrEmpty(puuid);
            var safeMatchId = Uri.EscapeDataString(matchId);


            var matchDetails = await GetMatchDetails(safeMatchId);

            var player = matchDetails.Players.FirstOrDefault(p => p.Puuid == puuid);

            return player?.Stats ?? new PlayerStats ();
        }

        public async Task<AccountInfo> GetAccountInfo(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);
            var url = $"https://api.henrikdev.xyz/valorant/v2/account/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);

            var jsonString = await response.Content.ReadAsStringAsync();


            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var raw = JsonSerializer.Deserialize<AccountResponseDto>(jsonString, options);

            return new AccountInfo
            {
                Card = raw.Data.Card ?? "",

                AccountLevel = raw.Data.AccountLevel,

                Puuid = raw.Data.Puuid ?? ""
            };

        }

        public async Task<List<PlayerPerformance>> GetRecentStats(List<string> matches, string puuid)
        {
            var stats = new List<PlayerPerformance>();


            matches = matches.Take(10).ToList();

            int count = 0;
            foreach (string match in matches)
            {
                _logger.LogInformation("Fetching match {match}", match);

                var matchStats = await GetPlayerStats(match, puuid);

                if (matchStats == null)
                {
                    continue;
                }

                var playerPerformance = _analyser.CalculateMatchStatistics(matchStats);

                stats.Add(playerPerformance);

                count++;

            }

            _logger.LogInformation("Stats created from {count} matches for user: {puuid}.", count, puuid);

            return stats;

        }

        public async Task<List<string>> GetMatchesByAgent(string gameName, string tagLine, string agentName)
        {

            List<string> matches = new List<string>();

            var loweredAgentName = agentName.ToLower();


            string puuid = await GetUserId(gameName, tagLine);

            var matchHistory = await GetMatches(gameName, tagLine);


            foreach (var match in matchHistory)
            {
                var matchDetails = await GetMatchDetails(match);

                bool hasAgent = matchDetails.Players.Any(p => p.Puuid == puuid && p.AgentName.ToLower() == loweredAgentName);

                if (hasAgent)
                {
                    matches.Add(matchDetails.MatchId);
                }
            }

            if (!matches.Any()) throw new NotFoundException("No matches found with this agent.");

            return matches;
        }

        

    }
}