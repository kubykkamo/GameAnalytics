using Microsoft.AspNetCore.Mvc;
using GameAnalytics.Data;
using GameAnalytics.Models;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;


namespace GameAnalytics.Controllers
{

    [ApiController]
    [Route("api/[controller]")]


    public class UsersController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
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


    }
}
 