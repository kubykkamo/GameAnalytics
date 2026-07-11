using Microsoft.AspNetCore.Mvc;
using GameAnalytics.Data;
using GameAnalytics.Models;
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

        private readonly AppDbContext _context;

        private readonly RiotApiService _riotApiService;

        public UsersController(AppDbContext context, RiotApiService riotApi)
        {
            _context = context;
            _riotApiService = riotApi;
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

        public async Task<IActionResult> GetPUUID(string gameName, string tagLine)
        {
            var puuid = await _riotApiService.GetPUUIDAsync(gameName, tagLine);
            if (puuid != null)
            {
                return Ok(puuid);
            }
            else
            {
                return NotFound("Player not found!");
            }
        }
    }
}
 