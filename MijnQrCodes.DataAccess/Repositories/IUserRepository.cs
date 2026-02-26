using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsername(string username);
    Task<User?> GetById(Guid id);
    Task<List<User>> GetAll();
    Task<User> Create(User user);
    Task<bool> UpdateApproval(Guid id, bool isApproved);
    Task<bool> UpdatePassword(Guid id, string passwordHash, bool mustChangePassword);
    Task<int> Count();
}
