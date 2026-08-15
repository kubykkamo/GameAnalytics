using GameAnalytics.Domain.Entities;
namespace GameAnalytics.Application 
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task<List<User>> GetAllAsync();
    }
    

}