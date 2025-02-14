using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Data.Repositories
{
    public class UserSettingsRepository : IUserSettings
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<User> _userManager;

        public UserSettingsRepository(ApplicationDbContext applicationDbContext, UserManager<User> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }
        public async Task<UserSettings> GetUserSettingsByUserIdAsync(string userId)
        {
            return await _applicationDbContext.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<UserSettingsDTO> GetUserSettingsDTOByUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && user.UserName != null)
            {
                var userSettings = await GetUserSettingsByUserIdAsync(userId);
                if (userSettings != null)
                {
                    var userSettingsDTO = new UserSettingsDTO
                    {
                        UserId = userSettings.UserId,
                        UserName = user.UserName,
                        Avatar = userSettings.Avatar,
                        AccountIsPublic = userSettings.AccountIsPublic,
                        HasPassword = !string.IsNullOrEmpty(user.PasswordHash)
                    };
                    return userSettingsDTO;
                }
            }
            return null;
        }

        public async Task UpdateUserSettings(UserSettings settings)
        {
            var userSettings = await GetUserSettingsByUserIdAsync(settings.UserId);
            if (userSettings != null)
            {
                userSettings.Avatar = userSettings.Avatar;
                userSettings.AccountIsPublic = userSettings.AccountIsPublic;
                _applicationDbContext.UserSettings.Update((UserSettings)userSettings);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
        public async Task UpdateUserSettingsByDTO(UserSettingsDTO settings)
        {
            var userSettings = await GetUserSettingsByUserIdAsync(settings.UserId);
            if (userSettings != null)
            {
                userSettings.Avatar = userSettings.Avatar;
                userSettings.AccountIsPublic = settings.AccountIsPublic;
                _applicationDbContext.UserSettings.Update((UserSettings)userSettings);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
