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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
