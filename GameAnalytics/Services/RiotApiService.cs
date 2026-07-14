using System.Text.Json;

using GameAnalytics.Models.External;
namespace GameAnalytics.Services
{
    public class RiotApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RiotApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var apiKey = _configuration["RiotApi:HenrikApiKey"];

            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);


        }

        public async Task<string?> GetUserId(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);
            var url = $"https://api.henrikdev.xyz/valorant/v2/account/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                return doc.RootElement.GetProperty("data").GetProperty("puuid").GetString();
            }
            return null;


        }
        public async Task<List<string?>> GetMatches(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);


            var url = $"https://api.henrikdev.xyz/valorant/v1/stored-matches/eu/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);





            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var matchList = JsonSerializer.Deserialize<MatchListDto>(jsonString, options);

                return matchList.Data.Select(m => m.Meta.Id).ToList();
            }


            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"HenrikDev API chyba: {response.StatusCode} - {error}");
        }

        public async Task<MatchResponseDto?> GetMatchDetails(string matchId)
        {
            var safeMatchId = Uri.EscapeDataString(matchId);
            var url = $"https://api.henrikdev.xyz/valorant/v4/match/eu/{safeMatchId}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var jsonString = await response.Content.ReadAsStringAsync();
                var matchDetails = JsonSerializer.Deserialize<MatchResponseDto>(jsonString, options);
                return matchDetails;
            }
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"HenrikDev API chyba: {response.StatusCode} - {error}");
        }

        public async Task<PlayerStatsDto?> GetPlayerStats(string matchId, string puuid)
        {
            if (string.IsNullOrEmpty(matchId) || string.IsNullOrEmpty(puuid))
            {
                return null;
            }

            var safeMatchId = Uri.EscapeDataString(matchId);
            

            var matchDetails = await GetMatchDetails(safeMatchId);

            if (matchDetails == null)
            {
                return null;
            }
            
            var player = matchDetails.Data.Players.FirstOrDefault(p => p.Puuid == puuid);

            if (player == null)
            {
                return null;
            }

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
    }
}