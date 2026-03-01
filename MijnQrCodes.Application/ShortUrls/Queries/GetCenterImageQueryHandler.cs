using MediatR;
using MijnQrCodes.Contracts.ShortUrls;
using MijnQrCodes.DataAccess.Repositories;

namespace MijnQrCodes.Application.ShortUrls.Queries;

public class GetCenterImageQueryHandler : IRequestHandler<GetCenterImageQuery, GetCenterImageResponse?>
{
    private readonly IShortUrlRepository _repository;

    public GetCenterImageQueryHandler(IShortUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetCenterImageResponse?> Handle(GetCenterImageQuery request, CancellationToken cancellationToken)
    {
        var (data, contentType) = await _repository.GetCenterImageData(request.ShortUrlId);
        if (data is null) return null;

        return new GetCenterImageResponse
        {
            ImageData = data,
            ContentType = contentType ?? "image/png"
        };
    }
}
