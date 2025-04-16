using NextGameAPI.Constants;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Services.DTOConverters
{
    public class GameSuggestionConverter
    {
        private readonly UserConverter _userConverter;
        public GameSuggestionConverter(UserConverter userConverter)
        {
            _userConverter = userConverter;
        }

        public List<GameSuggestionDTO> ConvertGameSuggestionsToDTOs (List<GameSuggestion> gameSuggestions)
        {
            var gameSuggestionDTOs = new List<GameSuggestionDTO>();
            foreach (var suggestion in gameSuggestions)
            {
                var newSuggestion = new GameSuggestionDTO
                {
                    Id = suggestion.Id,
                    CircleId = suggestion.CircleId,
                    GameId = suggestion.GameId,
                    GameName = suggestion.GameName,
                    GameCoverUrl = suggestion.GameCoverUrl,
                    SuggestedBy = suggestion.SuggestedBy,
                    Votes = ConvertGameVotesToDTOs(suggestion.Votes)
                };
                gameSuggestionDTOs.Add(newSuggestion);
            }
            return gameSuggestionDTOs;
        }

        public List<GameVoteDTO> ConvertGameVotesToDTOs (List<GameVote> gameVotes)
        {
            var gameVoteDTOs = new List<GameVoteDTO>();
            foreach (var vote in gameVotes)
            {
                var gameVoteDTO = new GameVoteDTO
                {
                    Id = vote.Id,
                    User = _userConverter.ConvertUserToUserDTO(vote.User),
                    Status = vote.Status,
                };
                gameVoteDTOs.Add(gameVoteDTO);
            }
            return gameVoteDTOs;
        }
    }
}
