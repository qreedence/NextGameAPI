using NextGameAPI.Data;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.Services.Transactions;

namespace NextGameAPI.Services.Circles
{
    public class CircleService
    {
        private readonly ICircle _circleRepository;
        private readonly ICircleMember _circleMemberRepository;
        private readonly TransactionService _transactionService;
        
        public CircleService(ICircle circleRepository, ICircleMember circleMemberRepository, TransactionService transactionService)
        {
            _circleRepository = circleRepository;
            _circleMemberRepository = circleMemberRepository;
            _transactionService = transactionService;
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

        public async Task InviteUserToCircle()
        {
            await Task.Delay(1000);
        }
    }
}

