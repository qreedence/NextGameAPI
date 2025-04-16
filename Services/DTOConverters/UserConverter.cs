using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;

namespace NextGameAPI.Services.DTOConverters
{
    public class UserConverter
    {
        private readonly ICircleInvitation _circleInvitationRepository;

        public UserConverter(ICircleInvitation circleInvitationRepository)
        {
            _circleInvitationRepository = circleInvitationRepository;
        }
        public UserDTO? ConvertUserToUserDTO(User user)
        {
            if (user != null)
            {
                var userDTO = new UserDTO
                {
                    UserId = user.Id,
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
                    UserId = user.Id,
                    Username = user.UserName!,
                    AccountIsPublic = user.Settings.AccountIsPublic,
                    Avatar = user.Settings.Avatar,
                }).ToList();
                return userDTOs;
            }
            return new List<UserDTO>();
        }

        public async Task<List<UserToInviteToCircleDTO>> ConvertUsersToUsersToInviteToCircleDTOs(List<User> users, Guid circleId)
        {
            if (users != null && users.Count > 0)
            {
                var userDTOs = new List<UserToInviteToCircleDTO>();
                foreach (var user in users)
                {
                    userDTOs.Add(new UserToInviteToCircleDTO
                    {
                        UserId = user.Id,
                        Username = user.UserName!,
                        AccountIsPublic = user.Settings.AccountIsPublic,
                        Avatar = user.Settings.Avatar,
                        InviteSent = await _circleInvitationRepository.CheckExisting(user, circleId) != null
                    });
                }
                return userDTOs;
            }
            return new List<UserToInviteToCircleDTO>();
        }
    }
}
