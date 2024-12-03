using MediatR;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace MijnQrCodes.Application.Queries.RenderQrCodeQuery;

internal class RenderQrCodeQueryHandler : IRequestHandler<RenderQrCodeQuery, RenderQrCodeResponse>
{
    // https://medium.com/@umairg404/generate-qr-codes-in-net-core-minimal-api-with-qrcoder-library-6eeeceb7e9aa

    private readonly IConfiguration _configuration;

    public RenderQrCodeQueryHandler(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<RenderQrCodeResponse> Handle(RenderQrCodeQuery query, CancellationToken cancellationToken)
    {
        var url = "https://www.involved.be";

        var qrGenerator = new QRCodeGenerator();
        var data = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.H);

        using var qrCode = new PngByteQRCode(data);

        return new RenderQrCodeResponse
        {
            QrCodeImage = qrCode.GetGraphic(20)
        };
    }
}