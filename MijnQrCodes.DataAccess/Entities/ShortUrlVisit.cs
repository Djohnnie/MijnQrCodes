namespace MijnQrCodes.DataAccess.Entities;

public class ShortUrlVisit
{
    public Guid Id { get; set; }
    public long SysId { get; set; }
    public Guid ShortUrlId { get; set; }
    public DateTime VisitedAt { get; set; }

    public ShortUrl ShortUrl { get; set; } = null!;
}
