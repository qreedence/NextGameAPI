using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

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

        [HttpPut]
        [Authorize]
        [EndpointName("MarkNotificationAsSeen")]
        [EndpointSummary("Marks a notification as seen.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkNotificationAsSeen(Guid id)
        {
            try
            {
                await _notificationRepo.MarkNotificationAsSeen(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("mark-all-as-seen")]
        [Authorize]
        [EndpointName("MarkAllNotificationsAsSeen")]
        [EndpointSummary("Marks all notifications as seen")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAllNotificationsAsSeen()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            try
            {
                await _notificationRepo.MarkAllNotificationsAsSeen(user.Id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize]
        [EndpointName("GetNotifications")]
        [EndpointSummary("Gets the logged in user's notifications.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NotificationDTO>))]
        public async Task<IActionResult> GetNotificationsAsync()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var notifications = await _notificationRepo.GetNotificationsForUser(user);
            if (notifications == null || notifications.Count == 0)
            {
                return Ok(new List<NotificationDTO>());
            }
            var notificationDTOs = notifications.Select(notification => new NotificationDTO
            {
                Id = notification.Id,
                Type = notification.Type, 
                Data = notification.Data,
                ActionUrl = notification.ActionUrl,
                Seen = notification.Seen,
                CreatedAt = notification.CreatedAt,
                AvatarUrl = notification.AvatarUrl,
            }).ToList();

            return Ok(notificationDTOs);
        }
    }
}
