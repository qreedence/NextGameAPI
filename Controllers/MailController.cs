using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Models;
using NextGameAPI.Services.Email;

namespace NextGameAPI.Controllers
{
    [Route("api/mail/")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly UserManager<User> _userManager;
        public MailController(EmailService emailService, UserManager<User> userManager)
        {
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SendTestEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email); 
            if (user != null)
            {
                try
                {
                    await _emailService.TestEmail(user);
                    return Ok();
                } 
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("User doesn't exist");
        }
    }
}
