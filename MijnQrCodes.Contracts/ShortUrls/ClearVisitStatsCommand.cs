using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class ClearVisitStatsCommand : IRequest<bool>
{
    public Guid ShortUrlId { get; set; }
}
