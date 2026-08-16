using Microsoft.AspNetCore.Mvc;
using GameAnalytics.Domain.Services;
using GameAnalytics.Domain.Entities;
using GameAnalytics.Infrastructure;
using GameAnalytics.Application;
using System.Text.RegularExpressions;



namespace GameAnalytics.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]


    public class UsersController(PlayerStatAnalyser _analyser, IUserRepository _context,
     IRiotApiClient _riotApiService, MatchAnalysisService _matchAnalyser) : ControllerBase
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
        public async Task<ActionResult<string>> GetId(string gameName, string tagLine)
        {
            var puuid = await _riotApiService.GetPlayerId(gameName, tagLine);

            return Ok(puuid);


        }


        [HttpGet("profile/{gameName}/{tagLine}")]

        public async Task<ActionResult<AccountInfo>> GetAccountInfo(string gameName, string tagLine)
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
        public async Task<ActionResult<MatchDetails>> GetMatchDetails(string matchId)
        {
            var matchDetails = await _riotApiService.GetMatchDetails(matchId);


            return Ok(matchDetails);
        }


        [HttpGet("match-details/{matchId}/player-statistics/{gameName}/{tagLine}")]

        public async Task<ActionResult<PlayerPerformance>> GetMatchStatistics(string matchId, string puuid)
        {

            var playerStats = await _riotApiService.GetPlayerStats(matchId, puuid);

            var matchStatistics = _analyser.CalculateMatchStatistics(playerStats);



            return Ok(matchStatistics);
        }


        [HttpGet("recent-stats/{gameName}/{tagLine}")]

        public async Task<ActionResult<PlayerPerformance>> GetRecentStats(string gameName, string tagLine)
        {
        
            var matches = await _riotApiService.GetMatches(gameName, tagLine);

            var stats = await _matchAnalyser.GetRecentStats(matches, gameName, tagLine);

            return Ok(stats);


        }

        [HttpGet("match-history/{gameName}/{tagLine}/{agentName}")]

        public async Task<ActionResult<List<string>>> GetMatchesByAgent(string gameName, string tagLine, string agentName)
        {
            var matches = await _matchAnalyser.GetMatchesByAgent(gameName, tagLine, agentName);

            return Ok(matches);
        }
    
    }
}
 