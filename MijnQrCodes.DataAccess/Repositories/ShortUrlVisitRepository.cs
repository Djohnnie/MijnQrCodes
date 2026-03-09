using Microsoft.EntityFrameworkCore;
using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess.Repositories;

public class ShortUrlVisitRepository : IShortUrlVisitRepository
{
    private readonly MijnQrCodesDbContext _dbContext;

    public ShortUrlVisitRepository(MijnQrCodesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RecordVisit(Guid shortUrlId)
    {
        _dbContext.ShortUrlVisits.Add(new ShortUrlVisit
        {
            Id = Guid.NewGuid(),
            ShortUrlId = shortUrlId,
            VisitedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<ShortUrlVisit>> GetVisits(Guid shortUrlId)
    {
        return await _dbContext.ShortUrlVisits
            .AsNoTracking()
            .Where(v => v.ShortUrlId == shortUrlId)
            .OrderBy(v => v.VisitedAt)
            .ToListAsync();
    }

    public async Task<int> GetTotalVisits(Guid shortUrlId)
    {
        return await _dbContext.ShortUrlVisits
            .AsNoTracking()
            .CountAsync(v => v.ShortUrlId == shortUrlId);
    }

    public async Task<List<ShortUrlVisit>> GetAllVisits()
    {
        return await _dbContext.ShortUrlVisits
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task ClearVisits(Guid shortUrlId)
    {
        await _dbContext.ShortUrlVisits
            .Where(v => v.ShortUrlId == shortUrlId)
            .ExecuteDeleteAsync();
    }
}
