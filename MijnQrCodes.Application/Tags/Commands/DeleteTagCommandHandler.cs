using MediatR;
using MijnQrCodes.Contracts.Tags;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.Tags.Commands;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, bool>
{
    private readonly ITagRepository _tagRepository;

    public DeleteTagCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<bool> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        return await _tagRepository.Delete(request.Id);
    }
}
