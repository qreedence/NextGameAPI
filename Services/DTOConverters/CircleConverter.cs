using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Services.DTOConverters
{
    public class CircleConverter
    {
        private readonly UserConverter _userConverter;
        public CircleConverter (UserConverter userConverter)
        {
            _userConverter = userConverter;
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
                    CreatedBy = _userConverter.ConvertUserToUserDTO(circle.CreatedBy) ?? new UserDTO { Username = "Unknown user" },
                };
                return circleDTO;
            }
            return null;
        }
    }
}
