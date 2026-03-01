using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class CreateShortUrlCommand : IRequest<ShortUrlDto>
{
    public string Title { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public string ForegroundColor { get; set; } = "#212121";
    public string FinderPatternColor { get; set; } = "#212121";
    public List<Guid> TagIds { get; set; } = [];
    public byte[]? CenterImageData { get; set; }
    public string? CenterImageContentType { get; set; }
}
