using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IUser
    {
        Task<List<User>> SearchUsersAsync(string userName);
    }
}
