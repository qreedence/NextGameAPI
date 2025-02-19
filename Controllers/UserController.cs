using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUser _userRepository;
        private readonly IFriendship _friendshipRepo;

        public UserController(IUser userRepository, IFriendship friendshipRepo, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _friendshipRepo = friendshipRepo;
            _userManager = userManager;
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
    }
}
