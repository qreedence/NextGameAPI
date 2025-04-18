using NextGameAPI.Constants;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
using NextGameAPI.DTOs.Circles;

namespace NextGameAPI.Services.DTOConverters
{
    public class UserConverter
    {
        private readonly ICircleInvitation _circleInvitationRepository;
        private readonly ICircleMember _circleMemberRepository;

        public UserConverter(ICircleInvitation circleInvitationRepository, ICircleMember circleMemberRepository)
        {
            _circleInvitationRepository = circleInvitationRepository;
            _circleMemberRepository = circleMemberRepository;
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

        public async Task<CircleMember?> ConvertCircleMemberDTOToCircleMember(CircleMemberDTO circleMemberDTO)
        {
            var circleMember = await _circleMemberRepository.GetByCircleIdAndUserIdAsync(circleMemberDTO.CircleId, circleMemberDTO.User.UserId);
            if (circleMember != null)
            {
                return circleMember;
            }
            return null;
        }

        public async Task<List<CircleMember>> ConvertCircleMemberDTOsToCircleMember(List<CircleMemberDTO> circleMemberDTOs)
        {
            if (circleMemberDTOs != null && circleMemberDTOs.Count > 0)
            {
                var circleMembers = new List<CircleMember>();
                foreach (var memberDTO in circleMemberDTOs)
                {
                    var circleMember = await _circleMemberRepository.GetByCircleIdAndUserIdAsync(memberDTO.CircleId, memberDTO.User.UserId);
                    if (circleMember != null)
                    {
                        circleMembers.Add(circleMember);
                    }
                }
                return circleMembers;
            }
            return new List<CircleMember>();
        }

        public CircleMemberDTO ConvertCircleMemberToCircleMemberDTO(CircleMember circleMember)
        {
            return new CircleMemberDTO
            {
                User = ConvertUserToUserDTO(circleMember.User),
                Role = circleMember.Role,
                JoinedAt = circleMember.JoinedAt,
                LeftAt = circleMember.LeftAt,
                CircleId = circleMember.Circle.Id,
            };
        }

        public List<CircleMemberDTO> ConvertCircleMembersToCircleMemberDTOs(List<CircleMember> circleMembers)
        {
            var circleMemberDTOs = new List<CircleMemberDTO>();
            if (circleMembers == null || circleMembers.Count == 0)
            {
                return circleMemberDTOs;
            }

            foreach (var circleMember in circleMembers)
            {
                var circleMemberDTO = ConvertCircleMemberToCircleMemberDTO(circleMember);
                circleMemberDTOs.Add(circleMemberDTO);
            }
            return circleMemberDTOs;
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

        public UserDTO ConvertCircleMemberToUserDTO(CircleMember circleMember)
        {
            return new UserDTO
            {
                UserId = circleMember.User.Id,
                Username = circleMember.User.UserName,
                Avatar = circleMember.User.Settings.Avatar,
            };
        }

        public List<UserDTO> ConvertCircleMembersToUserDTOs(List<CircleMember> circleMembers)
        {
            var users = new List<UserDTO>();
            foreach (var  circleMember in circleMembers)
            {
                var userDTO = ConvertCircleMemberToUserDTO (circleMember);
                users.Add(userDTO);
            }
            return users;
        }
    }
}
