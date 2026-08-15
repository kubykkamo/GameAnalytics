using GameAnalytics.Application;
using GameAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameAnalytics.Infrastructure
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User> AddAsync(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await context.Users.ToListAsync();
        }
    }
}