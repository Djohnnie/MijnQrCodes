namespace MijnQrCodes.DataAccess.Entities;

public class ShortUrlTag
{
    public Guid ShortUrlId { get; set; }
    public Guid TagId { get; set; }

    public ShortUrl ShortUrl { get; set; }
    public Tag Tag { get; set; }
}
