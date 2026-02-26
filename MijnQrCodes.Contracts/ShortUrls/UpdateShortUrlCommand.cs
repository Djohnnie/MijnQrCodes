using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class UpdateShortUrlCommand : IRequest<ShortUrlDto?>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
}
