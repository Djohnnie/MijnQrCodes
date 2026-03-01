namespace MijnQrCodes.Contracts.ShortUrls;

public class GetShortUrlsResponse
{
    public List<ShortUrlDto> ShortUrls { get; set; } = [];
}

public class ShortUrlDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public string ForegroundColor { get; set; } = "#212121";
    public string FinderPatternColor { get; set; } = "#212121";
    public List<ShortUrlTagDto> Tags { get; set; } = [];
    public bool HasCenterImage { get; set; }
    public string? CenterImageColor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ShortUrlTagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6366f1";
}
