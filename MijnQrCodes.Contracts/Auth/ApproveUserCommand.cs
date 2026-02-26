using MediatR;

namespace MijnQrCodes.Contracts.Auth;

public class ApproveUserCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
}
