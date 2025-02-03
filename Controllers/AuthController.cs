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
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EndpointName("LoginUser")]
        [EndpointSummary("Allows a user to log in with credentials provided")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login attempt.");
            }

            var user = await _userManager.FindByNameAsync(loginDTO.UserNameOrEmail) 
                    ?? await _userManager.FindByEmailAsync(loginDTO.UserNameOrEmail);

            if (user == null)
            {
                return Unauthorized($"Login failed.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, loginDTO.RememberMe, true);
            if (result.Succeeded)
            {
                return Ok();
            }
            return Unauthorized("Login failed.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            return Ok("Log out here");
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
