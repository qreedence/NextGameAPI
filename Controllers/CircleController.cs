using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public CircleController(CircleService circleService, UserManager<User> userManager)
        {
            _circleService = circleService;
            _userManager = userManager;
        }

        [HttpPost("create")]
        [Authorize]
        [EndpointName("CreateCircle")]
        [EndpointDescription("Lets a user create a circle")]
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
    }
}
