using MediatR;

namespace MijnQrCodes.Contracts.Auth;

public class GetUsersQuery : IRequest<GetUsersResponse>
{
}

public class GetUsersResponse
{
    public List<UserDto> Users { get; set; } = [];
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public bool MustChangePassword { get; set; }
    public DateTime CreatedAt { get; set; }
}
