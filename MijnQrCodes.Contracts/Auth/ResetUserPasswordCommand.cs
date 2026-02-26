using MediatR;

namespace MijnQrCodes.Contracts.Auth;

public class ResetUserPasswordCommand : IRequest<ResetUserPasswordResponse>
{
    public Guid UserId { get; set; }
}

public class ResetUserPasswordResponse
{
    public bool Success { get; set; }
    public string TemporaryPassword { get; set; } = string.Empty;
}
