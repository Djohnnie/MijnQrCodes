using MediatR;
using MijnQrCodes.Contracts.ShortUrls;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.ShortUrls.Commands;

public class ClearVisitStatsCommandHandler : IRequestHandler<ClearVisitStatsCommand, bool>
{
    private readonly IShortUrlVisitRepository _visitRepository;

    public ClearVisitStatsCommandHandler(IShortUrlVisitRepository visitRepository)
    {
        _visitRepository = visitRepository;
    }

    public async Task<bool> Handle(ClearVisitStatsCommand request, CancellationToken cancellationToken)
    {
        await _visitRepository.ClearVisits(request.ShortUrlId);
        return true;
    }
}
