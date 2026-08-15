using Microsoft.EntityFrameworkCore;
using GameAnalytics.Domain.Entities;

namespace GameAnalytics.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
