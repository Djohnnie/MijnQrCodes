using MediatR;
using MijnQrCodes.Application.Services;
using MijnQrCodes.Contracts.Auth;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Auth.Queries;

public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public LoginQueryHandler(IUserRepository userRepository, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsername(request.Username);
        if (user is null || !_passwordService.Verify(request.Password, user.PasswordHash))
        {
            return new LoginResponse { Success = false, Error = "Ongeldige gebruikersnaam of wachtwoord." };
        }

        if (!user.IsApproved)
        {
            return new LoginResponse { Success = false, Error = "Je account wacht nog op goedkeuring." };
        }

        return new LoginResponse
        {
            Success = true,
            UserId = user.Id,
            Username = user.Username,
            MustChangePassword = user.MustChangePassword
        };
    }
}
