using NextGameAPI.Constants;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs.Circles;

namespace NextGameAPI.Services.DTOConverters
{
    public class CircleGameConverter
    {
        private readonly UserConverter _userConverter;
        public CircleGameConverter(UserConverter userConverter)
        {
            _userConverter = userConverter;
        }

        public CircleGameDTO ConvertCircleGameToCircleGameDTO(CircleGame circleGameDTO)
        {
            return new CircleGameDTO
            {
                Id = circleGameDTO.Id,
                    GameId = circleGameDTO.GameId,
                    GameName = circleGameDTO.GameName,
                    GameCoverUrl = circleGameDTO.GameCoverUrl,
                    Players = _userConverter.ConvertCircleMembersToUserDTOs(circleGameDTO.Players),
                    DisplayOrder = circleGameDTO.DisplayOrder,
                    GameStatus = circleGameDTO.GameStatus,
                    DateAdded = circleGameDTO.DateAdded,
                    DateStarted = circleGameDTO.DateStarted,
                    DateFinished = circleGameDTO.DateFinished,
                    DatesPlayed = circleGameDTO.DatesPlayed,
                    SuggestedBy = _userConverter.ConvertCircleMemberToUserDTO(circleGameDTO.SuggestedBy),
                };
        }

        public async Task<List<CircleGameDTO>> ConvertCircleGamesToCircleGameDTOs(List<CircleGame> circleGames)
        {
            var circleGameDTOs = new List<CircleGameDTO>();
            if (circleGames == null || circleGames.Count <= 0)
            {
                return circleGameDTOs;
            }

            foreach (var circleGame in circleGames)
            {
                var circleGameDTO = ConvertCircleGameToCircleGameDTO(circleGame);
                circleGameDTOs.Add(circleGameDTO);
            }

            return circleGameDTOs;
        }

        public CircleGame ConvertCircleGameDTOToCircleGame(CircleGameDTO circleGameDTO)
        {
            throw new NotImplementedException();

        }

        public List<CircleGame> ConvertCircleGameDTOsToCircleGames(List<CircleGameDTO> circleGameDTOs)
        {
            throw new NotImplementedException();

        }
    }
}
