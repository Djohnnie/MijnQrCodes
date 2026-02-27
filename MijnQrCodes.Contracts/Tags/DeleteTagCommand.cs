using MediatR;

namespace MijnQrCodes.Contracts.Tags;

public class DeleteTagCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
