using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Services.DTOConverters
{
    public class CircleConverter
    {
        private readonly UserConverter _userConverter;
        private readonly GameSuggestionConverter _gameSuggestionConverter;
        public CircleConverter (UserConverter userConverter, GameSuggestionConverter gameSuggestionConverter)
        {
            _userConverter = userConverter;
            _gameSuggestionConverter = gameSuggestionConverter;
        }
        public CircleDTO? ConvertCircleToCircleDTO(Circle circle)
        {
            if (circle != null && circle.CreatedBy != null)
            {
                var circleDTO = new CircleDTO
                {
                    Id = circle.Id,
                    Name = circle.Name,
                    CreatedAt = circle.CreatedAt,
                    CreatedBy = _userConverter.ConvertUserToUserDTO(circle.CreatedBy) ?? new UserDTO {UserId="Unknown user", Username = "Unknown user" },
                    SuggestionQueue = _gameSuggestionConverter.ConvertGameSuggestionsToDTOs(circle.SuggestionQueue)
                };
                return circleDTO;
            }
            return null;
        }
    }
}
