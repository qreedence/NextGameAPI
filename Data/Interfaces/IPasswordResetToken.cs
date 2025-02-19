using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IPasswordResetToken
    {
        Task<PasswordResetToken> GetById(string id);
        Task AddPasswordResetToken(PasswordResetToken token);
        Task RemovePasswordResetToken(string id);
    }
}
