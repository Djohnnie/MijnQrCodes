using MediatR;
using MijnQrCodes.Contracts.ShortUrls;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.ShortUrls.Queries;

public class GetCenterImageQueryHandler : IRequestHandler<GetCenterImageQuery, byte[]?>
{
    private readonly IShortUrlRepository _repository;

    public GetCenterImageQueryHandler(IShortUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<byte[]?> Handle(GetCenterImageQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCenterImageData(request.ShortUrlId);
    }
}
