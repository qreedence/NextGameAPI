using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IExternalLoginToken
    {
        public Task<ExternalLoginToken> GetByIdAsync(Guid id);
        public Task Add(ExternalLoginToken externalLoginToken);
        public Task DeleteAsync(Guid tokenId);
    }
}
