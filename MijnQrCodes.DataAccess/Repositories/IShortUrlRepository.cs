using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess.Repositories;

public interface IShortUrlRepository
{
    Task<List<ShortUrl>> GetAll();
    Task<ShortUrl?> GetById(Guid id);
    Task<ShortUrl?> GetByShortCode(string shortCode);
    Task<ShortUrl> Create(ShortUrl shortUrl);
    Task<ShortUrl?> Update(ShortUrl shortUrl);
    Task<bool> Delete(Guid id);
    Task<bool> ShortCodeExists(string shortCode);
}
