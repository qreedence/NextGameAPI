using NextGameAPI.Data;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
using NextGameAPI.Services.DTOConverters;
using NextGameAPI.Services.Notifications;
using NextGameAPI.Services.Transactions;

namespace NextGameAPI.Services.Circles
{
    public class CircleService
    {
        private readonly ICircle _circleRepository;
        private readonly ICircleMember _circleMemberRepository;
        private readonly ICircleInvitation _circleInvitationRepository;
        private readonly IGameSuggestion _gameSuggestionRepository;
        private readonly NotificationService _notificationService;
        private readonly TransactionService _transactionService;
        private readonly UserConverter _userConverter;
        private readonly CircleConverter _circleConverter;
        private readonly IUser _userRepository;
        
        public CircleService(
            ICircle circleRepository,
            ICircleMember circleMemberRepository, 
            ICircleInvitation circleInvitationRepository,
            IGameSuggestion gameSuggestionRepository,
            NotificationService notificationService, 
            TransactionService transactionService, 
            UserConverter userConverter, 
            CircleConverter circleConverter, 
            IUser userRepository)
        {
            _circleRepository = circleRepository;
            _circleMemberRepository = circleMemberRepository;
            _circleInvitationRepository = circleInvitationRepository;
            _gameSuggestionRepository = gameSuggestionRepository;
            _transactionService = transactionService;
            _notificationService = notificationService;
            _userConverter = userConverter;
            _circleConverter = circleConverter;
            _userRepository = userRepository;
        }

        public async Task CreateCircle(User user, string name)
        {
            if (user != null && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(user.UserName))
            {
                await _transactionService.ExecuteInTransactionAsync(async () =>
                {
                    var circle = new Circle
                    {
                        Name = name,
                        CreatedBy = user,
                    };
                    await _circleRepository.CreateCircleAsync(circle);
                    var circleMember = await _circleMemberRepository.CreateCircleMemberAsync(circle.Id, user.UserName, Constants.CircleMemberRole.Owner);
                    circle.CircleMembers.Add(circleMember);
                });
            }
        }

        public async Task SuggestGameToCircle(Guid circleId, int gameId, string gameName, string gameCoverUrl, string suggestedByUserName)
        {
            var circle = await _circleRepository.GetByIdAsync(circleId);
            if (circle == null)
            {
                return;
            }

            if (circle.SuggestionQueue.Any(gs => gs.GameId == gameId))
            {
                return;
            }

            var gameSuggestion = new GameSuggestion
            {
                CircleId = circle.Id,
                GameId = gameId,
                GameName = gameName,
                GameCoverUrl = gameCoverUrl,
                SuggestedBy = suggestedByUserName,
            };

            await _gameSuggestionRepository.AddAsync(gameSuggestion);
            circle.SuggestionQueue.Add(gameSuggestion);
            await _circleRepository.UpdateCircleAsync(circle);
            
        }

        public async Task<List<GameSuggestion>> GetGameSuggestionsForCircleAsync(Guid circleId)
        {
            var circle = await _circleRepository.GetByIdAsync(circleId);
            if (circle != null)
            {
                return circle.SuggestionQueue;
            }
            return new List<GameSuggestion>();
        }

        public async Task<List<UserToInviteToCircleDTO>> FindFriendsToInviteToCircle(List<string> friendIds, List<string> existingMemberIds, string userNameFilter, Guid circleId)
        {
            var friends = await _userRepository.GetEligibleFriendsForCircleAsync(friendIds, existingMemberIds, userNameFilter);
            if (friends == null ||  friends.Count == 0)
            {
                return new List<UserToInviteToCircleDTO>();
            }
            return await _userConverter.ConvertUsersToUsersToInviteToCircleDTOs(friends, circleId);
        }

        public async Task InviteUserToCircle(User from, User to, Guid circleId)
        {
            var circle = await _circleRepository.GetByIdAsync(circleId);
            if (circle != null)
            {
                var invitation = await _circleInvitationRepository.CheckExisting(to, circleId);
                if (invitation == null)
                {
                    await _transactionService.ExecuteInTransactionAsync(async () =>
                    {
                        var circleInvitation = await _circleInvitationRepository.Create(from, to, circle);
                        var notification = await _notificationService.CreateCircleInvitationNotificationAsync(circleInvitation);
                        await _notificationService.SendNotificationAsync(to, notification);
                    });
                }
            }
        }

        public async Task<CircleInvitationDTO?> GetCircleInvitationDTOAsync(int id)
        {
            var circleInvitation = await _circleInvitationRepository.GetById(id);
            if (circleInvitation != null && circleInvitation.From != null)
            {
                var circleInvitationDTO = new CircleInvitationDTO
                {
                    Id = circleInvitation.Id,
                    From = _userConverter.ConvertUserToUserDTO(circleInvitation.From) ?? new UserDTO { Username = "Unknown user" },
                    Circle = _circleConverter.ConvertCircleToCircleDTO(circleInvitation.Circle),
                    SentAt = circleInvitation.SentAt,
                };
                return circleInvitationDTO;
            }
            return null;
        }

        public async Task<CircleInvitationDTO?> GetCircleInvitationByCircleIdAndUserIdAsync(Guid circleId, string userId)
        {
            var circleInvitation = await _circleInvitationRepository.GetByCircleIdAndUserIdAsync(circleId, userId);
            if (circleInvitation != null)
            {
                var circleInvitationDTO = new CircleInvitationDTO
                {
                    Id = circleInvitation.Id,
                    From = _userConverter.ConvertUserToUserDTO(circleInvitation.From) ?? new UserDTO { Username="Unknown user"},
                    Circle = _circleConverter.ConvertCircleToCircleDTO(circleInvitation.Circle),
                    SentAt = circleInvitation.SentAt,
                };
                return circleInvitationDTO;
            }
            return null;
        }

        public async Task CircleInvitationResponse(User user, int circleInvitationId, bool response)
        {
            if (circleInvitationId > 0 && user != null)
            {
                var circleInvitation = await _circleInvitationRepository.GetById(circleInvitationId);
                if (circleInvitation != null && user.Id == circleInvitation.To.Id)
                {
                    if (response)
                    {
                        await _transactionService.ExecuteInTransactionAsync(async () =>
                        {
                            var circleMember = await _circleMemberRepository.CreateCircleMemberAsync(circleInvitation.Circle.Id, circleInvitation.To.UserName!, Constants.CircleMemberRole.Member);
                            circleInvitation.Circle.CircleMembers.Add(circleMember);
                            await _circleInvitationRepository.Delete(circleInvitation.Id);
                        });
                    }
                    if (!response)
                    {
                        await _transactionService.ExecuteInTransactionAsync(async () =>
                        {
                            await _circleInvitationRepository.Delete(circleInvitation.Id);
                        });
                    }
                }
            }
        }

        public async Task<List<Circle>> GetCirclesByUserAsync(string userId)
        {
            return await _circleRepository.GetCirclesByUserId(userId);
        }

        public async Task<Circle> GetCircleByIdAsync(Guid circleId)
        {
            return await _circleRepository.GetByIdAsync(circleId);
        }

        public List<CircleDTO> ConvertCirclesToCircleDTOs(List<Circle> circles)
        {
            if (circles != null && circles.Count > 0)
            {
                var circleDTOs = circles.Select(circle => new CircleDTO
                {
                    Id = circle.Id,
                    Name = circle.Name,
                    CreatedAt = circle.CreatedAt,
                    CreatedBy = _userConverter.ConvertUserToUserDTO(circle.CreatedBy),
                    ActiveMembers = _userConverter.ConvertUsersToUserDTOs(circle.CircleMembers.Select(cm => cm.User).ToList()),
                    SuggestionQueue = circle.SuggestionQueue
                }).ToList();
                return circleDTOs;
            }
            return new List<CircleDTO>();
        }

        public async Task<(bool, string)> LeaveCircleAsync(User user, Guid circleId)
        {
            var circle = await _circleRepository.GetByIdAsync(circleId);
            if (circle != null)
            {
                var circleMember = await _circleMemberRepository.GetByCircleIdAndUserIdAsync(circleId, user.Id);
                if (circleMember != null && circleMember.Role == Constants.CircleMemberRole.Owner)
                {
                    return (false, "Unable to leave circle. Give ownership of the circle to someone else or delete the circle.");
                }
                await _transactionService.ExecuteInTransactionAsync(async () =>
                {
                    if (circleMember != null)
                    {
                        circleMember.IsActive = false;
                        circleMember.LeftAt = DateTime.UtcNow;
                        await _circleMemberRepository.UpdateCircleMemberAsync(circleMember);
                    }
                    
                });
                return (true, "");
            }
            return (false, "Can't find circle.");
        }
    }
}

