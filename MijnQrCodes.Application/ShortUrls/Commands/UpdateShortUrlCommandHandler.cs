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
            OriginalUrl = request.OriginalUrl,
            BackgroundColor = request.BackgroundColor,
            ForegroundColor = request.ForegroundColor,
            FinderPatternColor = request.FinderPatternColor
        };

        var updated = await _repository.Update(entity);
        if (updated is null) return null;

        await _repository.SetTags(updated.Id, request.TagIds);
        updated = await _repository.GetById(updated.Id);

        return new ShortUrlDto
        {
            Id = updated!.Id,
            Title = updated.Title,
            OriginalUrl = updated.OriginalUrl,
            ShortCode = updated.ShortCode,
            BackgroundColor = updated.BackgroundColor,
            ForegroundColor = updated.ForegroundColor,
            FinderPatternColor = updated.FinderPatternColor,
            Tags = updated.ShortUrlTags.Select(t => new ShortUrlTagDto
            {
                Id = t.Tag.Id,
                Name = t.Tag.Name,
                Color = t.Tag.Color
            }).ToList(),
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };
    }
}
