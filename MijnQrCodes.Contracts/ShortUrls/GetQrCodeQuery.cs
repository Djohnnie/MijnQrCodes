using MediatR;

namespace MijnQrCodes.Contracts.ShortUrls;

public class GetQrCodeQuery : IRequest<GetQrCodeResponse>
{
    public string Url { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public string ForegroundColor { get; set; } = "#212121";
    public string FinderPatternColor { get; set; } = "#212121";
}
