using MediatR;

namespace MijnQrCodes.Application.Queries.ResolveLinkQuery;

public class ResolveLinkQuery : IRequest<ResolveLinkResponse>
{
    public string Code { get; set; }
}