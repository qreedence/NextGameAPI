using NextGameAPI.Constants;
using NextGameAPI.Data;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
using NextGameAPI.DTOs.Circles;
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
        private readonly IGameVote _gameVoteRepository;
        private readonly ICircleGame _circleGameRepository;
        private readonly NotificationService _notificationService;
        private readonly TransactionService _transactionService;
        private readonly UserConverter _userConverter;
        private readonly CircleConverter _circleConverter;
        private readonly GameSuggestionConverter _gameSuggestionConverter;
        private readonly CircleGameConverter _circleGameConverter;
        private readonly IUser _userRepository;
        
        public CircleService(
            ICircle circleRepository,
            ICircleMember circleMemberRepository, 
            ICircleInvitation circleInvitationRepository,
            IGameSuggestion gameSuggestionRepository,
            IGameVote gameVoteRepository,
            ICircleGame circleGameRepository,
            NotificationService notificationService, 
            TransactionService transactionService, 
            UserConverter userConverter, 
            CircleConverter circleConverter, 
            GameSuggestionConverter gameSuggestionConverter,
            CircleGameConverter circleGameConverter,
            IUser userRepository)
        {
            _circleRepository = circleRepository;
            _circleMemberRepository = circleMemberRepository;
            _circleInvitationRepository = circleInvitationRepository;
            _gameSuggestionRepository = gameSuggestionRepository;
            _gameVoteRepository = gameVoteRepository;
            _circleGameRepository = circleGameRepository;
            _transactionService = transactionService;
            _notificationService = notificationService;
            _userConverter = userConverter;
            _circleConverter = circleConverter;
            _gameSuggestionConverter = gameSuggestionConverter;
            _circleGameConverter = circleGameConverter;
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

        public async Task VoteOnSuggestedGame(int gameSuggestionId, GameVoteStatus gameVoteStatus, User user)
        {
            if (gameSuggestionId <= 0 || user == null)
            {
                return;
            }

            var gameSuggestion = await _gameSuggestionRepository.GetByIdAsync(gameSuggestionId);
            if (gameSuggestion == null)
            {
                return;
            }

            if (gameSuggestion.Votes.FirstOrDefault(v => v.User?.Id == user?.Id) != null)
            {
                return;
            }

            var gameVote = new GameVote
            {
                Status = gameVoteStatus,
                User = user,
            };

            await _gameVoteRepository.AddAsync(gameVote);
            gameSuggestion.Votes.Add(gameVote);
            await _gameSuggestionRepository.UpdateAsync(gameSuggestion);
        }

        public async Task AddGameToCircle(AddGameToCircleRequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                return;
            }

            var circle = await _circleRepository.GetByIdAsync(requestDTO.CircleId);

            if (circle.CircleGames.Any(g => g.Id == requestDTO.GameId))
            {
                return;
            }

            var players = new List<User>();
            var circleMembers = new List<CircleMember>();
            foreach (var player in requestDTO.Players)
            {
                var user = await _userRepository.FindByUsernameAsync(player);
                if (user != null)
                {
                    var circleMember = await _circleMemberRepository.GetByCircleIdAndUserIdAsync(requestDTO.CircleId, user.Id);
                    if (circleMember != null)
                    {
                        circleMembers.Add(circleMember);
                    }
                }
            }

            var suggestedByUser = await _userRepository.FindByUsernameAsync(requestDTO.SuggestedBy);
            

            var game = new CircleGame
            {
                Circle = await _circleRepository.GetByIdAsync(requestDTO.CircleId),
                GameId = requestDTO.GameId,
                GameName = requestDTO.GameName,
                GameCoverUrl = requestDTO.GameCoverUrl,
                Players = circleMembers,
                DisplayOrder = circle.CircleGames.Count == 0
                  ? 0
                  : circle.CircleGames.Max(g => g.DisplayOrder) + 1,
                SuggestedBy = await _circleMemberRepository.GetByCircleIdAndUserIdAsync(requestDTO.CircleId, suggestedByUser.Id),
                GameStatus = requestDTO.GameStatus,
            };

            if (circle == null) 
            {
                return;
            }

            await _circleGameRepository.AddAsync(game);

            var gameSuggestion = await _gameSuggestionRepository.GetByGameIdAsync(requestDTO.CircleId, requestDTO.GameId);
            if (gameSuggestion != null)
            {
                circle.SuggestionQueue.Remove(gameSuggestion);
                await _gameSuggestionRepository.DeleteAsync(gameSuggestion.Id);
            }
            circle.CircleGames.Add(game);
            await _circleRepository.UpdateCircleAsync(circle);
        }

        public async Task ChangeGameStatusAsync(int circleGameId, GameStatus gameStatus)
        {
            var circleGame = await _circleGameRepository.GetByIdAsync(circleGameId);
            if (circleGame != null)
            {
                circleGame.GameStatus = gameStatus;
                await _circleGameRepository.UpdateAsync(circleGame);
            }
        }

        public async Task<List<CircleGameDTO>> GetCircleGamesForCircle(Guid circleId)
        {
            var circle = await _circleRepository.GetByIdAsync(circleId);
            return await _circleGameConverter.ConvertCircleGamesToCircleGameDTOs(circle.CircleGames);
        }

        public async Task<List<GameSuggestionDTO>> GetGameSuggestionsForCircleAsync(Guid circleId)
        {
            var circle = await _circleRepository.GetByIdAsync(circleId);
            if (circle != null)
            {
                return _gameSuggestionConverter.ConvertGameSuggestionsToDTOs(circle.SuggestionQueue);
            }
            return new List<GameSuggestionDTO>();
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
                    From = _userConverter.ConvertUserToUserDTO(circleInvitation.From) ?? new UserDTO { UserId = "Unkown user", Username = "Unknown user" },
                    Circle = await _circleConverter.ConvertCircleToCircleDTO(circleInvitation.Circle),
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
                    From = _userConverter.ConvertUserToUserDTO(circleInvitation.From) ?? new UserDTO { UserId="Unknown user", Username="Unknown user"},
                    Circle = await _circleConverter.ConvertCircleToCircleDTO(circleInvitation.Circle),
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

