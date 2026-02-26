using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class GetShortUrlByIdQuery : IRequest<ShortUrlDto?>
{
    public Guid Id { get; set; }
}
