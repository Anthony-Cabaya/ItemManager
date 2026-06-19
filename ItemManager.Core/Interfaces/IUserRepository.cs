using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);

        Task<User?> ValidateUserAsync(string username, string password);
    }
}
