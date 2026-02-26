using MediatR;

namespace MijnQrCodes.Contracts.Auth;

public class RegisterCommand : IRequest<RegisterResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public bool AutoApproved { get; set; }
}
