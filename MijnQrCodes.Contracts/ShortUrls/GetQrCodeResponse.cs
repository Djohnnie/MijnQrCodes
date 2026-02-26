namespace MijnQrCodes.Contracts.ShortUrls;

public class GetQrCodeResponse
{
    public byte[] ImageData { get; set; } = [];
    public string ContentType { get; set; } = "image/png";
}
