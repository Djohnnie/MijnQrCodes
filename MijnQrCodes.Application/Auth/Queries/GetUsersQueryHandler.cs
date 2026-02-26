using MediatR;
using MijnQrCodes.Contracts.Auth;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Auth.Queries;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, GetUsersResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAll();

        return new GetUsersResponse
        {
            Users = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                IsApproved = u.IsApproved,
                MustChangePassword = u.MustChangePassword,
                CreatedAt = u.CreatedAt
            }).ToList()
        };
    }
}
