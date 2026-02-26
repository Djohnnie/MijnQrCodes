using MediatR;
using MijnQrCodes.Contracts.Auth;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Auth.Commands;

public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public ApproveUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.UpdateApproval(request.UserId, true);
    }
}
