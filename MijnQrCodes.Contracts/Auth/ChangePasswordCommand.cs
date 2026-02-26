using MediatR;

namespace MijnQrCodes.Contracts.Auth;

public class ChangePasswordCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}
