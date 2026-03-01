namespace MijnQrCodes.DataAccess.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }
    public long SysId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public string ForegroundColor { get; set; } = "#212121";
    public string FinderPatternColor { get; set; } = "#212121";
    public string? CenterImageColor { get; set; }
    public byte[]? CenterImageData { get; set; }
    public string? CenterImageContentType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<ShortUrlTag> ShortUrlTags { get; set; } = [];
}
