using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
using System.Security.Claims;

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

        [HttpPost("login/google")]
        [EndpointName("LoginGoogle")]
        public async Task<IActionResult> LoginGoogleAsync()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponseAsync()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return Unauthorized("Google login failed.");
            }
            var email = result.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User { Email = email };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return BadRequest(createResult.Errors);
                }
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Redirect(Environment.GetEnvironmentVariable("cors-client-https-url")!);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointName("LogoutUser")]
        [EndpointSummary("Allows a user to log out")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            Task signOut = _signInManager.SignOutAsync();
            if (signOut.IsCompletedSuccessfully)
            {
                return Ok();
            }
            return BadRequest();
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

        [HttpGet("ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EndpointName("Ping")]
        [EndpointSummary("Pings the server to check if the user is authorized.")]
        public async Task<IActionResult> PingAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(true);
            }
            return Ok(false);
        }
    }
}
