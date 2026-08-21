using System.Text.Json;
using Microsoft.Extensions.Logging;
using GameAnalytics.Domain.Entities;
using GameAnalytics.Domain.Exceptions;
using GameAnalytics.Application;
namespace GameAnalytics.Infrastructure;
    public class RiotApiService(
    HttpClient _httpClient,   
    ILogger<RiotApiService> _logger) : IRiotApiClient
    {
        

        

        public async Task<string> GetPlayerId(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);
            var url = $"https://api.henrikdev.xyz/valorant/v2/account/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);

            JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            
            if(!doc.RootElement.TryGetProperty("data", out var data) || !data.TryGetProperty("puuid", out var puuidProperty))
            {
                throw new NotFoundException($"Could not find a Valorant account for {gameName}#{tagLine}.");
            }
            var puuid = puuidProperty.GetString();

            if(string.IsNullOrEmpty(puuid))
            {
                _logger.LogWarning("External api returned an empty puuid for {gameName}#{tagLine}", safeGameName, safeTagLine);
                throw new NotFoundException($"Could not find a Valorant account for {gameName}#{tagLine}.");
            }
            _logger.LogInformation("Successfully fetched PUUID for {GameName}#{TagLine}", safeGameName, safeTagLine);
            
            return puuid;
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

            if (matchList?.Data == null) throw new InvalidOperationException("External api returned an unexpected empty match list.");

            var readyList = matchList.Data.Select(x => x.MetaData.Id).ToList();

           _logger.LogInformation("Found {Count} matches for player {GameName}#{TagLine}", readyList.Count, safeGameName, safeTagLine);

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

            if (raw?.Data is null) throw new InvalidOperationException("External api returned an unexpected empty match details.");

            var matchDetails = new MatchDetails
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
            

            _logger.LogInformation("Successfully fetched match: {MatchId}", matchId);
            return matchDetails;
        }

        public async Task<PlayerStats> GetPlayerStats(string matchId, string puuid)
        {
            ArgumentException.ThrowIfNullOrEmpty(matchId);
            var safeMatchId = Uri.EscapeDataString(matchId);
     
            var matchDetails = await GetMatchDetails(safeMatchId);

            var player = matchDetails.Players.FirstOrDefault(p => p.Puuid == puuid);

            var stats = player?.Stats ?? new PlayerStats ();

            _logger.LogInformation("Successfully fetched ");

            return stats;
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

             if (raw?.Data is null) throw new InvalidOperationException("External api returned an unexpected empty account details.");

            var accountInfo = new AccountInfo{
                Card = raw.Data.Card ?? "",

                AccountLevel = raw.Data.AccountLevel,

                Puuid = raw.Data.Puuid ?? ""
            };

            _logger.LogInformation("Successfully fetched account information about {GameName}#{TagLine}.", safeGameName, safeTagLine);

            return accountInfo;
        }



    }
