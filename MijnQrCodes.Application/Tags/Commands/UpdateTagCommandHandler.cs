using MediatR;
using MijnQrCodes.Contracts.Tags;
using MijnQrCodes.DataAccess.Entities;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Tags.Commands;

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, TagDto?>
{
    private readonly ITagRepository _tagRepository;

    public UpdateTagCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<TagDto?> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.Update(new Tag
        {
            Id = request.Id,
            Name = request.Name,
            Color = request.Color
        });

        if (tag == null) return null;

        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Color = tag.Color,
            CreatedAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt
        };
    }
}
