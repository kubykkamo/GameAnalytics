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

            var apiKey = _configuration["RiotApi:ApiKey"];
            Console.WriteLine($"Using API KEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEey: {apiKey}");

            _httpClient.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);
        }


        public async Task<string?> GetPUUIDAsync(string gameName, string tagLine)
        {

            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine); 

            var url = $"https://europe.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = System.Text.Json.JsonDocument.Parse(content);
                var puuid = json.RootElement.GetProperty("puuid").GetString();
                return puuid;
            }

            Console.WriteLine(response);
            return null;
        }
    }
}
