namespace MijnQrCodes.Application.Services;

public interface IQrCodeService
{
    byte[] GenerateQrCode(string content, int size = 1024,
        string backgroundColor = "#FFFFFF", string foregroundColor = "#212121",
        string finderPatternColor = "#212121", byte[]? centerImageData = null);
}
