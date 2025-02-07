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

        public async Task UpdateUserSettings(UserSettingsDTO userSettingsDTO)
        {
            var userSettings = await GetUserSettingsByUserIdAsync(userSettingsDTO.UserId);
            if (userSettings != null)
            {
                userSettings.Avatar = userSettingsDTO.Avatar;
                userSettings.AccountIsPublic = userSettingsDTO.AccountIsPublic;
                _applicationDbContext.UserSettings.Update(userSettings);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
