using MediatR;

namespace MijnQrCodes.Contracts.Tags;

public class CreateTagCommand : IRequest<TagDto>
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6366f1";
}
