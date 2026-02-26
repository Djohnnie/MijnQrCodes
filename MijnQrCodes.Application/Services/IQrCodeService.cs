namespace MijnQrCodes.Application.Services;

public interface IQrCodeService
{
    byte[] GenerateQrCode(string content, int size = 1024);
}
