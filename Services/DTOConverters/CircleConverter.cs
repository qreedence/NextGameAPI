using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
using NextGameAPI.DTOs.Circles;

namespace NextGameAPI.Services.DTOConverters
{
    public class CircleConverter
    {
        private readonly UserConverter _userConverter;
        private readonly GameSuggestionConverter _gameSuggestionConverter;
        private readonly CircleGameConverter _circleGameConverter;
        public CircleConverter (UserConverter userConverter, GameSuggestionConverter gameSuggestionConverter, CircleGameConverter circleGameConverter)
        {
            _userConverter = userConverter;
            _gameSuggestionConverter = gameSuggestionConverter;
            _circleGameConverter = circleGameConverter;
        }
        public async Task<CircleDTO?> ConvertCircleToCircleDTO(Circle circle)
        {
            if (circle != null && circle.CreatedBy != null)
            {
                var circleDTO = new CircleDTO
                {
                    Id = circle.Id,
                    Name = circle.Name,
                    CreatedAt = circle.CreatedAt,
                    CreatedBy = _userConverter.ConvertUserToUserDTO(circle.CreatedBy) ?? new UserDTO {UserId="Unknown user", Username = "Unknown user" },
                    SuggestionQueue = _gameSuggestionConverter.ConvertGameSuggestionsToDTOs(circle.SuggestionQueue),
                    CircleGames = await _circleGameConverter.ConvertCircleGamesToCircleGameDTOs(circle.CircleGames)
                };

                circleDTO.ActiveMembers = circle.CircleMembers
                    .Where(cm => cm.IsActive)
                    .Select(cm => new CircleMemberDTO
                    {
                        User = _userConverter.ConvertUserToUserDTO(cm.User)!,
                        Role = cm.Role,
                        JoinedAt = cm.JoinedAt,
                        LeftAt = cm.LeftAt,
                        CircleId = circle.Id
                        
                    })
                    .ToList();

                return circleDTO;
            }
            return null;
        }

        public async Task<List<CircleDTO>> ConvertCirclesToCircleDTOs(List<Circle> circles)
        {
            if (circles == null || circles.Count == 0)
                return new List<CircleDTO>();

            var circleDtoTasks = circles.Select(ConvertCircleToCircleDTO).ToList();
            var circleDTOs = await Task.WhenAll(circleDtoTasks);

            return circleDTOs.Where(dto => dto != null).ToList()!;
        }
    }
}
