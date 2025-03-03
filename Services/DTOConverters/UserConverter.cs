using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Services.DTOConverters
{
    public class UserConverter
    {
        public UserDTO? ConvertUserToUserDTO(User user)
        {
            if (user != null)
            {
                var userDTO = new UserDTO
                {
                    Username = user.UserName!,
                    AccountIsPublic = user.Settings.AccountIsPublic,
                    Avatar = user.Settings.Avatar,
                };
                return userDTO;
            }
            return null;
        }

        public List<UserDTO> ConvertUsersToUserDTOs(List<User> users)
        {
            if (users != null && users.Count > 0)
            {
                var userDTOs = users.Select(user => new UserDTO
                {
                    Username = user.UserName!,
                    AccountIsPublic = user.Settings.AccountIsPublic,
                    Avatar = user.Settings.Avatar,
                }).ToList();
                return userDTOs;
            }
            return new List<UserDTO>();
        }
    }
}
