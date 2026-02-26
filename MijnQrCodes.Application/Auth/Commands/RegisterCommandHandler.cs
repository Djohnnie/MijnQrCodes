using MediatR;
using MijnQrCodes.Application.Services;
using MijnQrCodes.Contracts.Auth;
using MijnQrCodes.DataAccess.Entities;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public RegisterCommandHandler(IUserRepository userRepository, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByUsername(request.Username);
        if (existing is not null)
        {
            return new RegisterResponse { Success = false, Error = "Gebruikersnaam is al in gebruik." };
        }

        var isFirstUser = await _userRepository.Count() == 0;

        var user = new User
        {
            Username = request.Username,
            PasswordHash = _passwordService.Hash(request.Password),
            IsApproved = isFirstUser,
            MustChangePassword = false
        };

        await _userRepository.Create(user);

        return new RegisterResponse
        {
            Success = true,
            AutoApproved = isFirstUser
        };
    }
}
