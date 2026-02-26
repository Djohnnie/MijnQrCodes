namespace MijnQrCodes.DataAccess.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }
    public long SysId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
