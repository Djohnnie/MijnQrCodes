using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class DeleteShortUrlCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
