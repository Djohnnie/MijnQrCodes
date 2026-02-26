using Microsoft.EntityFrameworkCore;
using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MijnQrCodesDbContext _dbContext;

    public UserRepository(MijnQrCodesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Username == username);
    }

    public async Task<User?> GetById(Guid id)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<User>> GetAll()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<User> Create(User user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<bool> UpdateApproval(Guid id, bool isApproved)
    {
        var rows = await _dbContext.Users
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.IsApproved, isApproved)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));

        return rows > 0;
    }

    public async Task<bool> UpdatePassword(Guid id, string passwordHash, bool mustChangePassword)
    {
        var rows = await _dbContext.Users
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.PasswordHash, passwordHash)
                .SetProperty(x => x.MustChangePassword, mustChangePassword)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));

        return rows > 0;
    }

    public async Task<int> Count()
    {
        return await _dbContext.Users.CountAsync();
    }
}
