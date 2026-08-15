using Microsoft.AspNetCore.Mvc;
using GameAnalytics.Domain.Services;
using GameAnalytics.Domain.Entities;
using GameAnalytics.Infrastructure;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using GameAnalytics.Application;



namespace GameAnalytics.Controllers
{

    [ApiController]
    [Route("api/[controller]")]


    public class UsersController(HttpClient client, PlayerStatAnalyser _analyser, UserRepository _context, IRiotApiClient _riotApiService) : ControllerBase
    {
       


        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] User user)
        {

            return Ok(await _context.AddAsync(user));
        }
        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.GetAllAsync();
        }

        [HttpGet("puuid/{gameName}/{tagLine}")]
        public async Task<ActionResult<string>> GetUserId(string gameName, string tagLine)
        {
            var puuid = await _riotApiService.GetUserId(gameName, tagLine);

            return Ok(puuid);


        }


        [HttpGet("profile/{gameName}/{tagLine}")]

        public async Task<ActionResult<AccountData>> GetAccountInfo(string gameName, string tagLine)
        {
            var data = await _riotApiService.GetAccountInfo(gameName, tagLine);

            return Ok(data);

        }



        [HttpGet("match-history/{gameName}/{tagLine}")]

        public async Task<ActionResult<List<string>>> GetMatches(string gameName, string tagLine)
        {
            var matches = await _riotApiService.GetMatches(gameName, tagLine);

            return Ok(matches);
        }



        [HttpGet("match-details/{matchId}")]
        public async Task<ActionResult<SingleMatchResponseDto>> GetMatchDetails(string matchId)
        {
            var matchDetails = await _riotApiService.GetMatchDetails(matchId);


            return Ok(matchDetails);
        }


        [HttpGet("match-details/{matchId}/player-statistics/{gameName}/{tagLine}")]

        public async Task<ActionResult<PlayerPerformance>> GetMatchStatistics(string matchId, string gameName, string tagLine)
        {

            var puuid = await _riotApiService.GetUserId(gameName, tagLine);

            var playerStats = await _riotApiService.GetPlayerStats(matchId, puuid);

            var matchStatistics = _analyser.CalculateMatchStatistics(playerStats);



            return Ok(matchStatistics);
        }


        [HttpGet("recent-stats/{gameName}/{tagLine}")]

        public async Task<ActionResult<PlayerPerformance>> GetRecentStats(string gameName, string tagLine)
        {
            var puuid = await _riotApiService.GetUserId(gameName, tagLine);

            var matches = await _riotApiService.GetMatches(gameName, tagLine);

            var stats = await _riotApiService.GetRecentStats(matches, puuid);

            return Ok(stats);


        }

        [HttpGet("match-history/{gameName}/{tagLine}/{agentName}")]

        public async Task<ActionResult<List<string>>> GetMatchesByAgent(string gameName, string tagLine, string agentName)
        {
            var matches = await _riotApiService.GetMatchesByAgent(gameName, tagLine, agentName);

            return Ok(matches);
        }
    
    }
}
 