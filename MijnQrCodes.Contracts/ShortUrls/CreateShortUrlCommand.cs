using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class CreateShortUrlCommand : IRequest<ShortUrlDto>
{
    public string Title { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
}
