using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private INotification _notificationRepo;
        private UserManager<User> _userManager;
        public NotificationController(INotification notificationRepo, UserManager<User> userManager)
        {
            _notificationRepo = notificationRepo;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotificationsAsync()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await _notificationRepo.GetNotificationsForUser(user));
        }
    }
}
