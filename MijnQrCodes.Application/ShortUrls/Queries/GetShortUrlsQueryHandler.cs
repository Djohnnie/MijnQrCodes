using MediatR;
using MijnQrCodes.Contracts.ShortUrls;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.ShortUrls.Queries;

public class GetShortUrlsQueryHandler : IRequestHandler<GetShortUrlsQuery, GetShortUrlsResponse>
{
    private readonly IShortUrlRepository _repository;
    private readonly IShortUrlVisitRepository _visitRepository;

    public GetShortUrlsQueryHandler(IShortUrlRepository repository, IShortUrlVisitRepository visitRepository)
    {
        _repository = repository;
        _visitRepository = visitRepository;
    }

    public async Task<GetShortUrlsResponse> Handle(GetShortUrlsQuery request, CancellationToken cancellationToken)
    {
        var shortUrls = await _repository.GetAll();
        var visitCounts = await _visitRepository.GetVisitCounts();

        return new GetShortUrlsResponse
        {
            ShortUrls = shortUrls.Select(x => new ShortUrlDto
            {
                Id = x.Id,
                Title = x.Title,
                OriginalUrl = x.OriginalUrl,
                ShortCode = x.ShortCode,
                BackgroundColor = x.BackgroundColor,
                ForegroundColor = x.ForegroundColor,
                FinderPatternColor = x.FinderPatternColor,
                HasCenterImage = x.CenterImageData is { Length: > 0 },
                CenterImageColor = x.CenterImageColor,
                VisitCount = visitCounts.GetValueOrDefault(x.Id),
                Tags = x.ShortUrlTags.Select(t => new ShortUrlTagDto
                {
                    Id = t.Tag.Id,
                    Name = t.Tag.Name,
                    Color = t.Tag.Color
                }).ToList(),
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).ToList()
        };
    }
}
