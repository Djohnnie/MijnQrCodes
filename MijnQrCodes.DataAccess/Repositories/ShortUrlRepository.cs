using Microsoft.EntityFrameworkCore;
using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess.Repositories;

public class ShortUrlRepository : IShortUrlRepository
{
    private readonly MijnQrCodesDbContext _dbContext;

    public ShortUrlRepository(MijnQrCodesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ShortUrl>> GetAll()
    {
        return await _dbContext.ShortUrls
            .Include(x => x.ShortUrlTags).ThenInclude(x => x.Tag)
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<ShortUrl?> GetById(Guid id)
    {
        return await _dbContext.ShortUrls
            .Include(x => x.ShortUrlTags).ThenInclude(x => x.Tag)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ShortUrl?> GetByShortCode(string shortCode)
    {
        return await _dbContext.ShortUrls
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ShortCode == shortCode);
    }

    public async Task<ShortUrl> Create(ShortUrl shortUrl)
    {
        shortUrl.Id = Guid.NewGuid();
        shortUrl.CreatedAt = DateTime.UtcNow;
        shortUrl.UpdatedAt = DateTime.UtcNow;

        _dbContext.ShortUrls.Add(shortUrl);
        await _dbContext.SaveChangesAsync();

        return shortUrl;
    }

    public async Task<ShortUrl?> Update(ShortUrl shortUrl)
    {
        var existing = await _dbContext.ShortUrls
            .Include(x => x.ShortUrlTags)
            .SingleOrDefaultAsync(x => x.Id == shortUrl.Id);
        if (existing is null) return null;

        existing.Title = shortUrl.Title;
        existing.OriginalUrl = shortUrl.OriginalUrl;
        existing.BackgroundColor = shortUrl.BackgroundColor;
        existing.ForegroundColor = shortUrl.ForegroundColor;
        existing.FinderPatternColor = shortUrl.FinderPatternColor;
        if (shortUrl.CenterImageData is not null || shortUrl.CenterImageContentType is null)
        {
            existing.CenterImageData = shortUrl.CenterImageData;
            existing.CenterImageContentType = shortUrl.CenterImageContentType;
        }
        existing.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> Delete(Guid id)
    {
        var rows = await _dbContext.ShortUrls
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();

        return rows > 0;
    }

    public async Task<bool> ShortCodeExists(string shortCode)
    {
        return await _dbContext.ShortUrls
            .AsNoTracking()
            .AnyAsync(x => x.ShortCode == shortCode);
    }

    public async Task SetTags(Guid shortUrlId, List<Guid> tagIds)
    {
        await _dbContext.ShortUrlTags
            .Where(x => x.ShortUrlId == shortUrlId)
            .ExecuteDeleteAsync();

        foreach (var tagId in tagIds)
        {
            _dbContext.ShortUrlTags.Add(new ShortUrlTag
            {
                ShortUrlId = shortUrlId,
                TagId = tagId
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<byte[]?> GetCenterImageData(Guid id)
    {
        return await _dbContext.ShortUrls
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => x.CenterImageData)
            .SingleOrDefaultAsync();
    }
}
