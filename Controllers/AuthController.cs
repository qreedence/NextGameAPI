using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
using System.Security.Claims;
using static Azure.Core.HttpHeader;
using System.Xml;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Google.Apis.Auth;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Sprache;
using Microsoft.Extensions.Options;
using NextGameAPI.Data.Interfaces;

namespace NextGameAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IExternalLoginToken _externalLoginTokenRepo;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IExternalLoginToken externalLoginTokenRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalLoginTokenRepo = externalLoginTokenRepo;
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

        [HttpPost("external-login")]
        [EndpointName("ExternalLogin")]
        [EndpointSummary("Start a sign-in process through external login provider.")]
        public async Task<IActionResult> ExternalLoginAsync(string loginProvider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalAuthCallback", "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            properties.Items["prompt"] = "login";
            properties.AllowRefresh = true;
            return Challenge(properties, "Google");
        }

        [HttpGet("external-auth-callback")]
        [EndpointName("ExternalAuthCallback")]
        [EndpointSummary("Handles the response from external login provider after a sign-in attempt.")]
        public async Task<IActionResult> ExternalAuthCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Unauthorized("Error loading external login information.");
            }
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var externalLoginToken = new ExternalLoginToken { LoginProvider = info.LoginProvider, ProviderKey=info.ProviderKey, Email=email};
            await _externalLoginTokenRepo.Add(externalLoginToken);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                await _userManager.CreateAsync(new User { Email = email, UserName = email });
            }
            user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Redirect($"{returnUrl}");
            }
            var createLogin = await _userManager.AddLoginAsync(user, new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName));
            if (!createLogin.Succeeded)
            {
                return Redirect($"{returnUrl}");
            }
            return Redirect($"{returnUrl}/login/external?token={externalLoginToken.Id}");

        }

        [HttpGet("external-auth-complete")]
        [EndpointName("ExternalAuthComplete")]
        [EndpointSummary("Handles the exchange of an token id to sign in a user that used an external login provider.")]
        public async Task<IActionResult> ExternalAuthComplete(Guid tokenId)
        {
            var loginToken = await _externalLoginTokenRepo.GetByIdAsync(tokenId);
            if (loginToken == null || string.IsNullOrEmpty(loginToken.Email) || loginToken.Expiry < DateTime.UtcNow)
            {
                return Unauthorized();
            }
            var signInResult = await _signInManager.ExternalLoginSignInAsync(loginToken.LoginProvider, loginToken.ProviderKey, true, true);
            if (signInResult.Succeeded)
            {
                return Ok();
            }
            if (signInResult.IsLockedOut || signInResult.IsNotAllowed)
            {
                return Unauthorized();
            }
            if (signInResult.RequiresTwoFactor)
            {
                //TODO: implement 2FA signin
            }
            return Ok();
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
                UserName = registerDTO.UserName,
            };
            
            var response = await _userManager.CreateAsync(newUser, registerDTO.Password);
            if (response.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, Constants.Roles.User);
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
            return Ok(User?.Identity?.IsAuthenticated);
        }

        [HttpGet("get-user-name")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EndpointName("GetUserName")]
        [EndpointSummary("Gets the name of the logged in user.")]
        public async Task<IActionResult> GetNameAsync()
        {
            return Ok(User?.Identity?.Name);
        }
    }
}
