﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
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

        [HttpGet]
        [Authorize]
        [EndpointName("GetCircleById")]
        [EndpointDescription("Get a circle by ID.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CircleDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCircleByIdAsync(Guid circleId)
        {
            var circle = await _circleService.GetCircleByIdAsync(circleId);
            if (circle == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }

            var userCircles = await _circleService.GetCirclesByUserAsync(user.Id);
            if (userCircles.FirstOrDefault(c => c.Id == circleId) == null)
            {
                return Unauthorized();
            }

            return Ok(_circleService.ConvertCirclesToCircleDTOs(new List<Circle>{circle}).FirstOrDefault());

        }

        [HttpGet("by-user")]
        [Authorize]
        [EndpointName("GetCirclesByUser")]
        [EndpointDescription("Get a list of circles which the user is a part of.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CircleDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCirclesByUserAsync()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }

            var circles = await _circleService.GetCirclesByUserAsync(user.Id);
            if (circles == null ||  circles.Count == 0)
            {
                return Ok(new List<CircleDTO>());
            }

            return Ok(_circleService.ConvertCirclesToCircleDTOs(circles));
            
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

        [HttpGet("find-friends-to-invite")]
        [Authorize]
        [EndpointName("FindFriendsToInvite")]
        [EndpointDescription("Find a list of friends to invite to a circle. Will not show friends that are already active members of the circle.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(List<UserToInviteToCircleDTO>))]
        public async Task<IActionResult> FindFriendsToInviteAsync(Guid circleId, string username)
        {
            var circle = await _circleService.GetCircleByIdAsync(circleId);
            if (circle == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }

            var friends = await _friendshipRepository.GetFriendsForUserAsync(user);
            if (friends == null || friends.Count <= 0) 
            {
                return Ok(new List<UserDTO>());
            }

            var friendIds = friends.Select(f => f.Id).ToList();
            var existingMemberIds = circle.CircleMembers
                .Where(cm => cm.IsActive)
                .Select(cm => cm.User.Id)
                .ToList();

            var friendDTOs = await _circleService.FindFriendsToInviteToCircle(friendIds, existingMemberIds, username, circle.Id);
            if (friendDTOs == null || friendDTOs.Count == 0)
            {
                return Ok(new List<UserDTO>());
            }
            return Ok(friendDTOs);
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

        [HttpGet("invitation-by-id")]
        [Authorize]
        [EndpointName("GetCircleInvitationById")]
        [EndpointDescription("Get a specific circle invitation by id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(CircleInvitationDTO))]
        public async Task<IActionResult> GetCircleInvitationByIdAsync(int circleInvitationId)
        {
            var circleInvitationDTO = await _circleService.GetCircleInvitationDTOAsync(circleInvitationId);
            if (circleInvitationDTO == null)
            {
                return NotFound();
            }
            return Ok(circleInvitationDTO);
        }

        [HttpGet("invitation")]
        [Authorize]
        [EndpointName("GetCircleInvitation")]
        [EndpointDescription("Get a circle invitation by circleId and user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(CircleInvitationDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCircleInvitationByCircleIdAndUserAsync(Guid circleId)
        {
            if (circleId == Guid.Empty)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }

            var circleInvitation = await _circleService.GetCircleInvitationByCircleIdAndUserIdAsync(circleId, user.Id);
            if (circleInvitation != null)
            {
                return Ok(circleInvitation);
            }

            return NotFound();
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

        [HttpPut("leave")]
        [Authorize]
        [EndpointName("LeaveCircle")]
        [EndpointDescription("Allow a user to leave a circle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LeaveCircleAsync(Guid circleId)
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return Unauthorized();
            }

            var circles = await _circleService.GetCirclesByUserAsync(user.Id);
            if (circles == null || circles.Count == 0 || circles.FirstOrDefault(c => c.Id == circleId) == null)
            {
                return NotFound();
            }

            try
            {
                var result = await _circleService.LeaveCircleAsync(user, circleId);
                if (result.Item1 == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result.Item2);
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
