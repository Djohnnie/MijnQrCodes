using MediatR;
using MijnQrCodes.Contracts.Tags;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Tags.Queries;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, GetTagsResponse>
{
    private readonly ITagRepository _tagRepository;

    public GetTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<GetTagsResponse> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAll();

        return new GetTagsResponse
        {
            Tags = tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList()
        };
    }
}
