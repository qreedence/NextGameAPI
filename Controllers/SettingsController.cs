using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Controllers
{
    [Route("api/settings")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserSettings _userSettingsRepo;

        public SettingsController(UserManager<User> userManager, IUserSettings userSettingsRepo)
        {
            _userManager = userManager;
            _userSettingsRepo = userSettingsRepo;
        }

        [HttpGet]
        [Authorize]
        [EndpointName("GetUserSettings")]
        [EndpointSummary("Allows a user to retrieve their settings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserSettingsDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserSettings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.UserName))
            {
                return NotFound();
            }
            var dto = await _userSettingsRepo.GetUserSettingsDTOByUserIdAsync(user.Id);
            if (dto != null)
            {
                return Ok(dto);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [EndpointName("UpdateUserSettings")]
        [EndpointSummary("Allows a user to update their settings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserSettings(UserSettingsDTO userSettingsDTO)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (userSettingsDTO.UserName != user.UserName)
                {
                    user.UserName = userSettingsDTO.UserName;
                    await _userManager.UpdateAsync(user);
                }
                await _userSettingsRepo.UpdateUserSettings(userSettingsDTO);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut("change-password")]
        [Authorize]
        [EndpointName("ChangePassword")]
        [EndpointSummary("Allows a user to change their password by providing the old password, a new password and a confirmation of the new password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (string.IsNullOrEmpty(changePasswordDTO.OldPassword))
                {
                    var userHasPassword = await _userManager.HasPasswordAsync(user);
                    if (userHasPassword)
                    {
                        return Unauthorized();
                    }
                    var setPassword = await _userManager.AddPasswordAsync(user, changePasswordDTO.NewPassword);
                    if (setPassword.Succeeded)
                    {
                        return Ok();
                    }
                    return BadRequest(setPassword.Errors);
                }
                var changePassword = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
                if (changePassword.Succeeded)
                {
                    return Ok();
                }
                return Unauthorized(changePassword.Errors);
            }
            return Unauthorized();
        }
    }
}
