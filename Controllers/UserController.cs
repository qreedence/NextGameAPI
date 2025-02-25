using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
using NextGameAPI.Services.Notifications;

namespace NextGameAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUser _userRepository;
        private readonly IFriendship _friendshipRepo;
        private readonly IFriendRequest _friendRequestRepo;
        private readonly NotificationService _notificationService;

        public UserController(IUser userRepository, IFriendship friendshipRepo, UserManager<User> userManager, IFriendRequest friendRequestRepo, NotificationService notificationService)
        {
            _userRepository = userRepository;
            _friendshipRepo = friendshipRepo;
            _userManager = userManager;
            _friendRequestRepo = friendRequestRepo;
            _notificationService = notificationService;
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointName("SearchUsers")]
        [EndpointSummary("Search for users with public accounts based on username.")]
        [Authorize]
        public async Task<IActionResult> SearchForUsersAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Username is required.");
            }
            var users = await _userRepository.SearchUsersAsync(userName);
            if (users != null &&  users.Count > 0)
            {
                var userDTOs = users.Select(user => new UserDTO
                {
                    Username = user.UserName!,
                    Avatar = user.Settings.Avatar
                }).ToList();
                return Ok(userDTOs);
            }
            return NotFound();
        }

        [HttpGet("friends")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointName("GetFriends")]
        [EndpointSummary("Get the friends for the signed in user.")]
        public async Task<IActionResult> GetFriendsForUser()
        {
            if (User == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }

            var friends = await _friendshipRepo.GetFriendsForUserAsync(user);
            if (friends.Count <= 0)
            {
               return NotFound();
            }
            var friendDTOs = friends.Select(friend => new UserDTO
            {
                Username = friend.UserName!,
                Avatar = friend.Settings.Avatar
            }).ToList();
            return Ok(friendDTOs);
        }

        [HttpGet("add-friend")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointName("AddFriend")]
        [EndpointSummary("Send a friend request.")]
        public async Task<IActionResult> AddFriend(string username)
        {
            if (User == null)
            {
                return Unauthorized();
            }
            var loggedInUser = await _userManager.FindByNameAsync(User.Identity?.Name);
            if (loggedInUser == null)
            {
                return BadRequest();
            }
            var outGoingFriendRequests = await _friendRequestRepo.OutgoingFriendRequests(loggedInUser.UserName);
            var userToSendFriendRequestTo = await _userManager.FindByNameAsync(username);
            if (userToSendFriendRequestTo == null)
            {
                return NotFound();
            }
            if (outGoingFriendRequests.Any(x => x.To == userToSendFriendRequestTo))
            {
                return BadRequest();
            }

            if (loggedInUser == userToSendFriendRequestTo)
            {
                return BadRequest();
            }

            

            try
            {
                await _friendRequestRepo.CreateFriendRequest(loggedInUser, userToSendFriendRequestTo);
                var notification = await _notificationService.CreateFriendRequestNotificationAsync(loggedInUser, userToSendFriendRequestTo);
                await _notificationService.SendNotificationAsync(userToSendFriendRequestTo, notification);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet("pending-friend-requests")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointName("GetPendingFriendRequests")]
        [EndpointSummary("Lets a user see their incoming pending friend requests.")]

        public async Task<IActionResult> GetPendingFriendRequestsAsync()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var friendRequests = await _friendRequestRepo.PendingFriendRequests(user.UserName!);
            if (friendRequests != null && friendRequests.Count > 0)
            {
                var friendRequestDTOs = friendRequests.Select(fr => new FriendRequestDTO
                {
                    From = fr.From.UserName,
                    To = fr.To.UserName,
                    SentAt = fr.SentAt,
                });
                return Ok(friendRequestDTOs);
            }
            return NoContent();
        }

        [HttpGet("outgoing-friend-requests")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointName("OutgoingFriendRequests")]
        [EndpointSummary("Lets a user see their outgoing friend requests.")]
        public async Task<IActionResult> GetOutgoingFriendRequestsAsync()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var friendRequests = await _friendRequestRepo.OutgoingFriendRequests(user.UserName!);
            if (friendRequests != null && friendRequests.Count > 0)
            {
                var friendRequestDTOs = friendRequests.Select(fr => new FriendRequestDTO
                {
                    From = fr.From.UserName,
                    To = fr.To.UserName,
                    SentAt = fr.SentAt,
                });
                return Ok(friendRequestDTOs);
            }
            return NoContent();
        }

        [HttpGet("friend-request-response")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointName("FriendRequestResponse")]
        [EndpointSummary("Lets a user respond to a friend request by either accepting or denying it.")]

        public async Task<IActionResult> FriendRequestResponseAsync(int friendRequestId, FriendRequestStatus status)
        {
            var friendRequest = await _friendRequestRepo.GetById(friendRequestId);
            if (friendRequest == null)
            {
                return NotFound();
            }

            if (friendRequest.To.UserName != User.Identity.Name)
            {
                return Unauthorized();
            }

            if (friendRequest.Status == FriendRequestStatus.Pending)
            {
                if (status == FriendRequestStatus.Accepted || status == FriendRequestStatus.Declined)
                {
                    friendRequest.Status = status;
                }
            }
            else
            {
                return BadRequest();
            }

            try
            {

                await _friendRequestRepo.UpdateFriendRequestAsync(friendRequest);
                if (friendRequest.Status == FriendRequestStatus.Accepted)
                {
                    var friendship = new Friendship
                    {
                        UserA = friendRequest.From,
                        UserB = friendRequest.To,

                    };
                    await _friendshipRepo.CreateFriendship(friendship);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }
    }
}
