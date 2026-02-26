using MediatR;
using MijnQrCodes.Application.Services;
using MijnQrCodes.Contracts.Auth;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Auth.Commands;

public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, ResetUserPasswordResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public ResetUserPasswordCommandHandler(IUserRepository userRepository, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<ResetUserPasswordResponse> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var tempPassword = _passwordService.GenerateTemporaryPassword();
        var hash = _passwordService.Hash(tempPassword);

        var success = await _userRepository.UpdatePassword(request.UserId, hash, mustChangePassword: true);

        return new ResetUserPasswordResponse
        {
            Success = success,
            TemporaryPassword = success ? tempPassword : string.Empty
        };
    }
}
