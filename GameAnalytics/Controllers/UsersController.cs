using Microsoft.AspNetCore.Mvc;
using GameAnalytics.Data;
using GameAnalytics.Models;
using GameAnalytics.Models.External;
using GameAnalytics.Models.Internal;
using System.Net.Http;
using GameAnalytics.Services;
using Microsoft.EntityFrameworkCore;


namespace GameAnalytics.Controllers
{

    [ApiController]
    [Route("api/[controller]")]


    public class UsersController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly PlayerStatAnalyser _analyser;

        private readonly AppDbContext _context;

        private readonly RiotApiService _riotApiService;

        public UsersController(AppDbContext context, RiotApiService riotApi, PlayerStatAnalyser analyser)
        {
            _context = context;
            _riotApiService = riotApi;
            _analyser = analyser;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] User user)
        {
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        [HttpGet("puuid/{gameName}/{tagLine}")]
        public async Task<IActionResult> GetUserId(string gameName, string tagLine) 
        {
            var puuid = await _riotApiService.GetUserId(gameName, tagLine);

            return Ok(puuid);
            
            
        }


        [HttpGet("profile/{gameName}/{tagLine}")]

        public async Task<IActionResult> GetAccountInfo(string gameName, string tagLine)
        { 
            var data = await _riotApiService.GetAccountInfo(gameName, tagLine);

            return Ok(data);
            
        }



        [HttpGet("match-history/{gameName}/{tagLine}")]

        public async Task<IActionResult> GetMatches(string gameName, string tagLine)
        {
            var matches = await _riotApiService.GetMatches(gameName, tagLine);
           
            return Ok(matches);
        }
            
        

        [HttpGet("match-details/{matchId}")]
        public async Task<IActionResult> GetMatchDetails(string matchId)
        {
            var matchDetails = await _riotApiService.GetMatchDetails(matchId);
        

            return Ok(matchDetails);
        }

        [HttpGet("match-details/{matchId}/player-statistics/{gameName}/{tagLine}")]

        public async Task<IActionResult> GetMatchStatistics(string matchId, string gameName, string tagLine)
        {
           
            var puuid = await _riotApiService.GetUserId(gameName, tagLine);

            var playerStats = await _riotApiService.GetPlayerStats(matchId, puuid);

            var matchStatistics = _analyser.CalculateMatchStatistics(playerStats);

             

            return Ok(matchStatistics);
        }
    }
}
 