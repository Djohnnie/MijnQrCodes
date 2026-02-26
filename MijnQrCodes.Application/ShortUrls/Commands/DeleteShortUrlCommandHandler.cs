using MediatR;
using MijnQrCodes.Contracts.ShortUrls;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.ShortUrls.Commands;

public class DeleteShortUrlCommandHandler : IRequestHandler<DeleteShortUrlCommand, bool>
{
    private readonly IShortUrlRepository _repository;

    public DeleteShortUrlCommandHandler(IShortUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteShortUrlCommand request, CancellationToken cancellationToken)
    {
        return await _repository.Delete(request.Id);
    }
}
