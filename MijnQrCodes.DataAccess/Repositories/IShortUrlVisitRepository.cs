using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess.Repositories;

public interface IShortUrlVisitRepository
{
    Task RecordVisit(Guid shortUrlId);
    Task<List<ShortUrlVisit>> GetVisits(Guid shortUrlId);
    Task<int> GetTotalVisits(Guid shortUrlId);
    Task<List<ShortUrlVisit>> GetAllVisits();
}
