using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class GetVisitStatsQuery : IRequest<GetVisitStatsResponse>
{
    public Guid ShortUrlId { get; set; }
}
