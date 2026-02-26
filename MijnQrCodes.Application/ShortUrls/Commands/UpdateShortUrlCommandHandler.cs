using MediatR;
using MijnQrCodes.Contracts.ShortUrls;
using MijnQrCodes.DataAccess.Entities;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.ShortUrls.Commands;

public class UpdateShortUrlCommandHandler : IRequestHandler<UpdateShortUrlCommand, ShortUrlDto?>
{
    private readonly IShortUrlRepository _repository;

    public UpdateShortUrlCommandHandler(IShortUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShortUrlDto?> Handle(UpdateShortUrlCommand request, CancellationToken cancellationToken)
    {
        var entity = new ShortUrl
        {
            Id = request.Id,
            Title = request.Title,
            OriginalUrl = request.OriginalUrl
        };

        var updated = await _repository.Update(entity);
        if (updated is null) return null;

        return new ShortUrlDto
        {
            Id = updated.Id,
            Title = updated.Title,
            OriginalUrl = updated.OriginalUrl,
            ShortCode = updated.ShortCode,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };
    }
}
