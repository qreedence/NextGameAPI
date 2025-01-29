using Microsoft.AspNetCore.Mvc;

namespace NextGameAPI.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet("noparam")]
        public async Task<IActionResult> TestAsync()
        {
            await Task.Delay(1000);
            return Ok("Hejsan");
        }
    }
}
