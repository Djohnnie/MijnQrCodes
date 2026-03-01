using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class GetCenterImageQuery : IRequest<GetCenterImageResponse?>
{
    public Guid ShortUrlId { get; set; }
}

public class GetCenterImageResponse
{
    public byte[] ImageData { get; set; } = [];
    public string ContentType { get; set; } = "image/png";
}
