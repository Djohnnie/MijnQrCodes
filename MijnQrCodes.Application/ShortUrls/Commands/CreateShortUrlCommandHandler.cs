using MediatR;
using MijnQrCodes.Contracts.ShortUrls;
using MijnQrCodes.DataAccess.Entities;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.ShortUrls.Commands;

public class CreateShortUrlCommandHandler : IRequestHandler<CreateShortUrlCommand, ShortUrlDto>
{
    private readonly IShortUrlRepository _repository;

    public CreateShortUrlCommandHandler(IShortUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShortUrlDto> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        var shortCode = await GenerateUniqueShortCode();

        var entity = new ShortUrl
        {
            Title = request.Title,
            OriginalUrl = request.OriginalUrl,
            ShortCode = shortCode,
            BackgroundColor = request.BackgroundColor,
            ForegroundColor = request.ForegroundColor,
            FinderPatternColor = request.FinderPatternColor,
            CenterImageData = request.CenterImageData,
            CenterImageContentType = request.CenterImageContentType,
            CenterImageColor = request.CenterImageColor
        };

        var created = await _repository.Create(entity);

        if (request.TagIds.Count > 0)
        {
            await _repository.SetTags(created.Id, request.TagIds);
            created = await _repository.GetById(created.Id);
        }

        return new ShortUrlDto
        {
            Id = created!.Id,
            Title = created.Title,
            OriginalUrl = created.OriginalUrl,
            ShortCode = created.ShortCode,
            BackgroundColor = created.BackgroundColor,
            ForegroundColor = created.ForegroundColor,
            FinderPatternColor = created.FinderPatternColor,
            HasCenterImage = created.CenterImageData is { Length: > 0 },
            CenterImageColor = created.CenterImageColor,
            Tags = created.ShortUrlTags.Select(t => new ShortUrlTagDto
            {
                Id = t.Tag.Id,
                Name = t.Tag.Name,
                Color = t.Tag.Color
            }).ToList(),
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }

    private async Task<string> GenerateUniqueShortCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();

        string shortCode;
        do
        {
            shortCode = new string(Enumerable.Range(0, 7)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        } while (await _repository.ShortCodeExists(shortCode));

        return shortCode;
    }
}
