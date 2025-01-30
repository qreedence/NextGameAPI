using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NextGameAPI.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet("noparam")]
        [Authorize]
        public async Task<IActionResult> TestAsync()
        {
            if (User != null)
            {
                await Task.Delay(1000);
                return Ok("Hejsan");
            }
            return Unauthorized();
        }
    }
}
