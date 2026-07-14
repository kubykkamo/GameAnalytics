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

            if (puuid != null) {
                return Ok(puuid);
            }
            else
            {
                return NotFound("User not found!");
            }
            
        }




        [HttpGet("match-history/{gameName}/{tagLine}")]

        public async Task<IActionResult> GetMatches(string gameName, string tagLine)
        {
            var matches = await _riotApiService.GetMatches(gameName, tagLine);
            if (matches != null)
            {
                return Ok(matches);
            }
            else
            {
                return NotFound("No matches found!");
            }
        }

        [HttpGet("match-details/{matchId}")]
        public async Task<IActionResult> GetMatchDetails(string matchId)
        {
            var matchDetails = await _riotApiService.GetMatchDetails(matchId);
            if (matchDetails != null)
            {
                return Ok(matchDetails);
            }
            else
            {
                return NotFound("Match details not found!");
            }
        }

        [HttpGet("match-details/{matchId}/player-statistics/{gameName}/{tagLine}")]

        public async Task<IActionResult> GetMatchStatistics(string matchId, string gameName, string tagLine)
        {
            if (string.IsNullOrEmpty(matchId) ||
                string.IsNullOrEmpty(gameName) ||
                string.IsNullOrEmpty(tagLine))
            {
                return BadRequest("Missing required parameters.");
            }

            var puuid = await _riotApiService.GetUserId(gameName, tagLine);

            if (puuid == null)
                return NotFound("Player not found.");

            var playerStats = await _riotApiService.GetPlayerStats(matchId, puuid);

            if (playerStats == null)
                return NotFound("Player statistics not found.");

            var matchStatistics = _analyser.CalculateMatchStatistics(playerStats);

            

            return Ok(matchStatistics);
        }
    }
}
 