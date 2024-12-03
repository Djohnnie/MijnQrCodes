using MediatR;
using Microsoft.Extensions.Configuration;

namespace MijnQrCodes.Application.Queries.ResolveLinkQuery;

internal class ResolveLinkQueryHandler : IRequestHandler<ResolveLinkQuery, ResolveLinkResponse>
{
    private readonly IConfiguration _configuration;

    public ResolveLinkQueryHandler(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<ResolveLinkResponse> Handle(ResolveLinkQuery query, CancellationToken cancellationToken)
    {
        return new ResolveLinkResponse
        {
            Url = "https://www.involved.be"
        };
    }
}