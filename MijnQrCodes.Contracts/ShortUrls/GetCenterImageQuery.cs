using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class GetCenterImageQuery : IRequest<byte[]?>
{
    public Guid ShortUrlId { get; set; }
}
