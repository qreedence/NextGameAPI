using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AuthController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync()
        {
            await Task.Delay(1000);
            return Ok("Log in here");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await Task.Delay(1000);
            return Ok("Log out here");
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [EndpointName("RegisterUser")]
        [EndpointSummary("Register a user")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterDTO registerDTO)
        {
            var newUser = new User
            {
                Email = registerDTO.Email,
                UserName = registerDTO.UserName
            };
            var response = await _userManager.CreateAsync(newUser, registerDTO.Password);
            if (response.Succeeded)
            {
                return Created();
            }
            return BadRequest(response.Errors);
        }
    }
}
