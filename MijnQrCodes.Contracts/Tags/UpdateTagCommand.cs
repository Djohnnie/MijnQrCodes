using MediatR;

namespace MijnQrCodes.Contracts.Tags;

public class UpdateTagCommand : IRequest<TagDto?>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6366f1";
}
