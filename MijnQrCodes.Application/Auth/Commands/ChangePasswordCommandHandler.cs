using MediatR;
using MijnQrCodes.Application.Services;
using MijnQrCodes.Contracts.Auth;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Auth.Commands;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var hash = _passwordService.Hash(request.NewPassword);
        return await _userRepository.UpdatePassword(request.UserId, hash, mustChangePassword: false);
    }
}
