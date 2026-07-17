using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using GameAnalytics.Exceptions;
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

        public async Task<string> GetUserId(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);
            var url = $"https://api.henrikdev.xyz/valorant/v2/account/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new NotFoundException("User does not exist.");
            }

            JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("data").GetProperty("puuid").GetString();
            

        }
        public async Task<List<string>> GetMatches(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);


            var url = $"https://api.henrikdev.xyz/valorant/v1/stored-matches/eu/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);


            if (!response.IsSuccessStatusCode)
            {
                throw new NotFoundException("No matches found.");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var matchList = JsonSerializer.Deserialize<MatchListDto>(jsonString, options);

            if (matchList == null)
            {
                throw new NotFoundException("Matches not found.");
            }

            var readyList =  matchList.Data.Select(x => x.Meta.Id).ToList();

            return readyList;
        }

       
        

        public async Task<MatchResponseDto> GetMatchDetails(string matchId)
        {
            var safeMatchId = Uri.EscapeDataString(matchId);
            var url = $"https://api.henrikdev.xyz/valorant/v4/match/eu/{safeMatchId}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            { 
                throw new NotFoundException("Match does not exist.");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonString = await response.Content.ReadAsStringAsync();
            var matchDetails = JsonSerializer.Deserialize<MatchResponseDto>(jsonString, options);

            if(matchDetails == null)
            {
                throw new NotFoundException("Match details unavailable.");
            }    

            return matchDetails;
            
            
        }

        public async Task<PlayerStatsDto> GetPlayerStats(string matchId, string puuid)
        {
            if (string.IsNullOrEmpty(matchId))
            {
                throw new ArgumentException("matchId cannot be empty", nameof(matchId));
            }

            if (string.IsNullOrEmpty(puuid))
            {
                throw new ArgumentException("puuid cannot be empty", nameof(puuid));
            }
            var safeMatchId = Uri.EscapeDataString(matchId);
            

            var matchDetails = await GetMatchDetails(safeMatchId);

            if (matchDetails == null)
            {
                throw new NotFoundException("Match not found.");
            }
            
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

            if (!response.IsSuccessStatusCode) 
            { 
                throw new NotFoundException("User does not exist."); 
            }
            
            var jsonString = await response.Content.ReadAsStringAsync();


            var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
            var accountDetails = JsonSerializer.Deserialize<AccountResponseDto>(jsonString, options);
 
            if (accountDetails == null) 
            {
                throw new NotFoundException("Account not found.");
            }


            return new AccountData
            {
                Card = accountDetails.Data.Card ?? "",

                AccountLevel = accountDetails.Data.AccountLevel,

                Puuid = accountDetails.Data.Puuid ?? ""
            };

        }
    }
}