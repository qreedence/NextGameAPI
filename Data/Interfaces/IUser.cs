using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IUser
    {
        Task<User?> FindByUsernameAsync(string username);
        Task<List<User>> SearchUsersAsync(string username);
    }
}
