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


        public async Task<string?> GetMatches(string gameName, string tagLine)
        {
            var safeGameName = Uri.EscapeDataString(gameName);
            var safeTagLine = Uri.EscapeDataString(tagLine);

            
            var url = $"https://api.henrikdev.xyz/valorant/v4/matches/eu/pc/{safeGameName}/{safeTagLine}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

           
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"HenrikDev API chyba: {response.StatusCode} - {error}");
        }
    }
}
