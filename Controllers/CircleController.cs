using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.Services.Circles;

namespace NextGameAPI.Controllers
{
    [Route("api/circle")]
    [ApiController]
    public class CircleController : ControllerBase
    {
        private readonly CircleService _circleService;
        private readonly UserManager<User> _userManager;
        private readonly IFriendship _friendshipRepository;

        public CircleController(CircleService circleService, UserManager<User> userManager, IFriendship friendshipRepository)
        {
            _circleService = circleService;
            _userManager = userManager;
            _friendshipRepository = friendshipRepository;
        }

        [HttpPost("create")]
        [Authorize]
        [EndpointName("CreateCircle")]
        [EndpointDescription("Lets a user create a circle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCircleAsync(string circleName)
        {
            if (!string.IsNullOrEmpty(User?.Identity?.Name))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null && !string.IsNullOrEmpty(circleName))
                {
                    try
                    {
                        await _circleService.CreateCircle(user, circleName);
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }
            return BadRequest();
        }

        [HttpPost("invite")]
        [Authorize]
        [EndpointName("InviteToCircle")]
        [EndpointDescription("Lets a user invite a friend a circle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InviteToCircleAsync(Guid circleId, string username)
        {
            if (!string.IsNullOrEmpty(User?.Identity?.Name) && !string.IsNullOrEmpty(username) && circleId != Guid.Empty)
            {
                var from = await _userManager.FindByNameAsync(User.Identity.Name);
                var to = await _userManager.FindByNameAsync(username);
                if (from != null && to != null)
                {
                    var checkFriendship = await _friendshipRepository.CheckExistingFriendshipAsync(from, to);
                    if (checkFriendship.Item1 == true)
                    {
                        await _circleService.InviteUserToCircle(from, to, circleId);
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }

        [HttpPost("invitation-response")]
        [Authorize]
        [EndpointName("InvitationResponse")]
        [EndpointDescription("Let a user respond to a circle invitation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CircleInvitationResponseAsync(int circleInvitationId, bool response)
        {
            if (!string.IsNullOrEmpty(User?.Identity?.Name))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    await _circleService.CircleInvitationResponse(user, circleInvitationId, response);
                    return Ok();
                }
            }
            return BadRequest();
        }
    }
}
