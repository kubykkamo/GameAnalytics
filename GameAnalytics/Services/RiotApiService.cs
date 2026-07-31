using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using GameAnalytics.Exceptions;
using GameAnalytics.Models.External;
using GameAnalytics.Models.Internal;
namespace GameAnalytics.Services
{
    public class RiotApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly PlayerStatAnalyser _analyser;
        private readonly ILogger<RiotApiService> _logger;

        public RiotApiService(HttpClient httpClient, IConfiguration configuration, PlayerStatAnalyser analyser, ILogger<RiotApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _analyser = analyser;
            _logger = logger;
            var apiKey = _configuration["RiotApi:HenrikApiKey"];

            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);


        }

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

        public async Task<SingleMatchResponseDto> GetMatchDetails(string matchId)
        {
            var safeMatchId = Uri.EscapeDataString(matchId);
            var url = $"https://api.henrikdev.xyz/valorant/v4/match/eu/{safeMatchId}";
            var response = await _httpClient.GetAsync(url);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonString = await response.Content.ReadAsStringAsync();
            var matchDetails = JsonSerializer.Deserialize<SingleMatchResponseDto>(jsonString, options);

            return matchDetails;


        }

        public async Task<PlayerStatsDto> GetPlayerStats(string matchId, string puuid)
        {
            if (string.IsNullOrEmpty(matchId))
            {
                throw new ArgumentException("matchId cannot be empty", matchId);
            }

            if (string.IsNullOrEmpty(puuid))
            {
                throw new ArgumentException("puuid cannot be empty", nameof(puuid));
            }
            var safeMatchId = Uri.EscapeDataString(matchId);


            var matchDetails = await GetMatchDetails(safeMatchId);

            var player = matchDetails.Data.Players.FirstOrDefault(p => p.Puuid == puuid);

            return new PlayerStatsDto
            {
                Kills = player?.Stats.Kills ?? 0,
                Deaths = player?.Stats.Deaths ?? 0,
                Assists = player?.Stats.Assists ?? 0,
                Headshots = player?.Stats.Headshots ?? 0,
                Bodyshots = player?.Stats.Bodyshots ?? 0,
                Legshots = player?.Stats.Legshots ?? 0

            };




        }

        public async Task<AccountData> GetAccountInfo(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);
            var url = $"https://api.henrikdev.xyz/valorant/v2/account/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);

            var jsonString = await response.Content.ReadAsStringAsync();


            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var accountDetails = JsonSerializer.Deserialize<AccountResponseDto>(jsonString, options);

            return new AccountData
            {
                Card = accountDetails.Data.Card ?? "",

                AccountLevel = accountDetails.Data.AccountLevel,

                Puuid = accountDetails.Data.Puuid ?? ""
            };

        }

        public async Task<List<PlayerPerformanceDto>> GetRecentStats(List<string> matches, string puuid)
        {
            var stats = new List<PlayerPerformanceDto>();


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

                bool hasAgent = matchDetails.Data.Players.Any(p => p.Puuid == puuid && p.Agent.Name.ToLower() == loweredAgentName);

                if (hasAgent)
                {
                    matches.Add(matchDetails.Data.Metadata.MatchId);
                }
            }

            if (!matches.Any()) throw new NotFoundException("No matches found with this agent.");

            return matches;
        }

    }
}