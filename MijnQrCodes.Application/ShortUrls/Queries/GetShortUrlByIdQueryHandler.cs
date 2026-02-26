using MediatR;
using MijnQrCodes.Contracts.ShortUrls;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.ShortUrls.Queries;

public class GetShortUrlByIdQueryHandler : IRequestHandler<GetShortUrlByIdQuery, ShortUrlDto?>
{
    private readonly IShortUrlRepository _repository;

    public GetShortUrlByIdQueryHandler(IShortUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShortUrlDto?> Handle(GetShortUrlByIdQuery request, CancellationToken cancellationToken)
    {
        var shortUrl = await _repository.GetById(request.Id);
        if (shortUrl is null) return null;

        return new ShortUrlDto
        {
            Id = shortUrl.Id,
            Title = shortUrl.Title,
            OriginalUrl = shortUrl.OriginalUrl,
            ShortCode = shortUrl.ShortCode,
            CreatedAt = shortUrl.CreatedAt,
            UpdatedAt = shortUrl.UpdatedAt
        };
    }
}
