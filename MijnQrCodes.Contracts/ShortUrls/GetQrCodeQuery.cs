using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class GetQrCodeQuery : IRequest<GetQrCodeResponse>
{
    public string Url { get; set; } = string.Empty;
}
