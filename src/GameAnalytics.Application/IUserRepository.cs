using GameAnalytics.Domain.Entities;

namespace GameAnalytics.Application
{
    
    public interface IUserRepository
    {
        
        Task<List<User>> GetAllAsync();
        Task<User> AddAsync(User user);
    }

}