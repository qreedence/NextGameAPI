using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.DTOs.Games;
using NextGameAPI.Services.IGDB;

namespace NextGameAPI.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("search/{searchTerm}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof (List<GameSearchResultDTO>))]
        public async Task<IActionResult> SearchGamesAsync([FromRoute]string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest();
            }

            var games = await _gameService.SearchGamesAsync(searchTerm);
            if (games == null ||  games.Count == 0)
            {
                return Ok(new List<GameSearchResultDTO>());
            }

            return Ok(games);
        }
    }
}
