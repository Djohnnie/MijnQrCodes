using MediatR;
using MijnQrCodes.Contracts.Tags;
using MijnQrCodes.DataAccess.Entities;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Tags.Commands;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
{
    private readonly ITagRepository _tagRepository;

    public CreateTagCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.Create(new Tag
        {
            Name = request.Name,
            Color = request.Color
        });

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
