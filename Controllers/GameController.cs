using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
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
        [EndpointName("Search")]
        [EndpointDescription("Search for a game by name.")]
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

        [HttpGet("new")]
        [EndpointName("New")]
        [EndpointDescription("Get new games.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof (List<GameSearchResultDTO>))]
        public async Task<IActionResult> GetNewGamesAsync()
        {
            var games = await _gameService.GetNewGamesAsync();
            if (games == null || games.Count == 0)
            {
                return Ok(new List<GameSearchResultDTO>());
            }
            return Ok(games);
        }

        [HttpGet("new/all")]
        [EndpointName("GetAllNew")]
        [EndpointDescription("Get all new games, paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GameSearchResultDTO>))]
        public async Task<IActionResult> GetAllNewGamesAsync(int page)
        {
            var games = await _gameService.GetAllNewGamesAsync(page);
            if (games == null || games.Count == 0)
            {
                return Ok(new List<GameSearchResultDTO>());
            }
            return Ok(games);
        }

        [HttpGet]
        [EndpointName("GetById")]
        [EndpointDescription("Get a specific game by ID.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGameByIdAsync(string gameId)
        {
            var game = await _gameService.GetGameAsync(gameId);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game);
        }

        [HttpGet("top")]
        [EndpointName("HighestRated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GameSearchResultDTO>))]
        public async Task<IActionResult> GetHighestRatedGamesAsync(int year)
        {
            var games = await _gameService.GetHighestRatedGamesOfYear(year);
            if (games == null || games.Count == 0)
            {
                return Ok(new List<GameSearchResultDTO>());
            }
            return Ok(games);
        }

        [HttpGet("top/all")]
        [EndpointName("GetAllHighestRated")]
        [EndpointDescription("Get all new highest rated games for a given year, paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GameSearchResultDTO>))]
        public async Task<IActionResult> GetAllHighestRatedGamesAsync(int year, int page)
        {
            var games = await _gameService.GetAllHighestRatedGamesOfYear(year, page, 50);
            if (games == null || games.Count == 0)
            {
                return Ok(new List<GameSearchResultDTO>());
            }
            return Ok(games);
        }
    }
}
