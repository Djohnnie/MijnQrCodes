using MediatR;
using MijnQrCodes.Application.Services;
using MijnQrCodes.Contracts.ShortUrls;

namespace MijnQrCodes.Application.ShortUrls.Queries;

public class GetQrCodeQueryHandler : IRequestHandler<GetQrCodeQuery, GetQrCodeResponse>
{
    private readonly IQrCodeService _qrCodeService;

    public GetQrCodeQueryHandler(IQrCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    public Task<GetQrCodeResponse> Handle(GetQrCodeQuery request, CancellationToken cancellationToken)
    {
        var imageData = _qrCodeService.GenerateQrCode(request.Url,
            backgroundColor: request.BackgroundColor,
            foregroundColor: request.ForegroundColor,
            finderPatternColor: request.FinderPatternColor,
            centerImageData: request.CenterImageData,
            centerImageColor: request.CenterImageColor);

        return Task.FromResult(new GetQrCodeResponse
        {
            ImageData = imageData,
            ContentType = "image/png"
        });
    }
}
