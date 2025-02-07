using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Data.Interfaces
{
    public interface IUserSettings
    {
        public Task<UserSettings> GetUserSettingsByUserIdAsync(string userId);
        public Task<UserSettingsDTO> GetUserSettingsDTOByUserIdAsync(string userId);
        public Task UpdateUserSettings(UserSettingsDTO userSettingsDTO);
    }
}
