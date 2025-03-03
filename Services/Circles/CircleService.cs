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
        private readonly NotificationService _notificationService;
        private readonly TransactionService _transactionService;
        private readonly UserConverter _userConverter;
        
        public CircleService(ICircle circleRepository, ICircleMember circleMemberRepository, ICircleInvitation circleInvitationRepository, NotificationService notificationService, TransactionService transactionService, UserConverter userConverter)
        {
            _circleRepository = circleRepository;
            _circleMemberRepository = circleMemberRepository;
            _circleInvitationRepository = circleInvitationRepository;
            _transactionService = transactionService;
            _notificationService = notificationService;
            _userConverter = userConverter;
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

        public async Task InviteUserToCircle(User from, User to, Guid circleId)
        {
            var circle = await _circleRepository.GetByIdAsync(circleId);
            if (circle != null)
            {
                await _transactionService.ExecuteInTransactionAsync(async () =>
                {
                    var circleInvitation = await _circleInvitationRepository.Create(from, to, circle);
                    var notification = await _notificationService.CreateCircleInvitationNotificationAsync(circleInvitation);
                    await _notificationService.SendNotificationAsync(to, notification);
                });
            }
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
                    ActiveMembers = _userConverter.ConvertUsersToUserDTOs(circle.CircleMembers.Select(cm => cm.User).ToList())
                }).ToList();
                return circleDTOs;
            }
            return new List<CircleDTO>();
        }

    }
}

