using MediatR;

namespace MijnQrCodes.Application.Queries.RenderQrCodeQuery;

public class RenderQrCodeQuery : IRequest<RenderQrCodeResponse>
{
    public string Code { get; set; }
}