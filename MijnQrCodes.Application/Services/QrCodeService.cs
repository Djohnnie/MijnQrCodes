using QRCoder;
using SkiaSharp;

namespace MijnQrCodes.Application.Services;

public class QrCodeService : IQrCodeService
{
    private const int QuietZoneModules = 2;

    public byte[] GenerateQrCode(string content, int size = 400)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
        var moduleData = qrCodeData.ModuleMatrix;
        var moduleCount = moduleData.Count;

        var totalModules = moduleCount + QuietZoneModules * 2;
        var moduleSize = (float)size / totalModules;

        using var surface = SKSurface.Create(new SKImageInfo(size, size));
        var canvas = surface.Canvas;

        canvas.Clear(SKColors.White);

        using var darkPaint = new SKPaint
        {
            Color = new SKColor(33, 33, 33),
            IsAntialias = false,
            Style = SKPaintStyle.Fill
        };

        for (var row = 0; row < moduleCount; row++)
        {
            for (var col = 0; col < moduleCount; col++)
            {
                if (!moduleData[row][col]) continue;

                if (IsFinderPatternArea(row, col, moduleCount)) continue;

                var x = (col + QuietZoneModules) * moduleSize;
                var y = (row + QuietZoneModules) * moduleSize;
                canvas.DrawRect(x, y, moduleSize, moduleSize, darkPaint);
            }
        }

        DrawRoundedFinderPattern(canvas, QuietZoneModules * moduleSize, QuietZoneModules * moduleSize, moduleSize);
        DrawRoundedFinderPattern(canvas, (moduleCount - 7 + QuietZoneModules) * moduleSize, QuietZoneModules * moduleSize, moduleSize);
        DrawRoundedFinderPattern(canvas, QuietZoneModules * moduleSize, (moduleCount - 7 + QuietZoneModules) * moduleSize, moduleSize);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }

    private static bool IsFinderPatternArea(int row, int col, int moduleCount)
    {
        // Top-left 7x7
        if (row < 7 && col < 7) return true;
        // Top-right 7x7
        if (row < 7 && col >= moduleCount - 7) return true;
        // Bottom-left 7x7
        if (row >= moduleCount - 7 && col < 7) return true;

        return false;
    }

    private static void DrawRoundedFinderPattern(SKCanvas canvas, float x, float y, float moduleSize)
    {
        var outerSize = 7 * moduleSize;
        var middleOffset = moduleSize;
        var middleSize = 5 * moduleSize;
        var innerOffset = 2 * moduleSize;
        var innerSize = 3 * moduleSize;
        var cornerRadius = moduleSize * 1.2f;

        using var whitePaint = new SKPaint { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Fill };
        using var darkPaint = new SKPaint { Color = new SKColor(33, 33, 33), IsAntialias = true, Style = SKPaintStyle.Fill };

        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x, y, x + outerSize, y + outerSize), cornerRadius), darkPaint);
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x + middleOffset, y + middleOffset, x + middleOffset + middleSize, y + middleOffset + middleSize), cornerRadius * 0.7f), whitePaint);
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x + innerOffset, y + innerOffset, x + innerOffset + innerSize, y + innerOffset + innerSize), cornerRadius * 0.5f), darkPaint);
    }
}
