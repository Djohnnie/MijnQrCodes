using System.Collections;
using QRCoder;
using SkiaSharp;

namespace MijnQrCodes.Application.Services;

public class QrCodeService : IQrCodeService
{
    private const int QuietZoneModules = 2;

    public byte[] GenerateQrCode(string content, int size = 1024)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
        var moduleData = qrCodeData.ModuleMatrix;
        var moduleCount = moduleData.Count;

        var totalModules = moduleCount + QuietZoneModules * 2;
        var moduleSize = (float)size / totalModules;
        var cornerRadius = moduleSize * 0.4f;

        using var surface = SKSurface.Create(new SKImageInfo(size, size));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        using var darkPaint = new SKPaint
        {
            Color = new SKColor(33, 33, 33),
            IsAntialias = true,
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

                var hasTop = IsDataModule(moduleData, row - 1, col, moduleCount);
                var hasBottom = IsDataModule(moduleData, row + 1, col, moduleCount);
                var hasLeft = IsDataModule(moduleData, row, col - 1, moduleCount);
                var hasRight = IsDataModule(moduleData, row, col + 1, moduleCount);

                DrawRoundedModule(canvas, x, y, moduleSize, cornerRadius,
                    hasTop, hasRight, hasBottom, hasLeft, darkPaint);
            }
        }

        DrawFinderPattern(canvas, QuietZoneModules * moduleSize, QuietZoneModules * moduleSize, moduleSize);
        DrawFinderPattern(canvas, (moduleCount - 7 + QuietZoneModules) * moduleSize, QuietZoneModules * moduleSize, moduleSize);
        DrawFinderPattern(canvas, QuietZoneModules * moduleSize, (moduleCount - 7 + QuietZoneModules) * moduleSize, moduleSize);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static bool IsDataModule(List<BitArray> moduleData, int row, int col, int moduleCount)
    {
        if (row < 0 || row >= moduleCount || col < 0 || col >= moduleCount) return false;
        if (IsFinderPatternArea(row, col, moduleCount)) return false;
        return moduleData[row][col];
    }

    private static bool IsFinderPatternArea(int row, int col, int moduleCount)
    {
        if (row < 7 && col < 7) return true;
        if (row < 7 && col >= moduleCount - 7) return true;
        if (row >= moduleCount - 7 && col < 7) return true;
        return false;
    }

    private static void DrawRoundedModule(SKCanvas canvas, float x, float y, float size, float radius,
        bool hasTop, bool hasRight, bool hasBottom, bool hasLeft, SKPaint paint)
    {
        var rect = new SKRect(x, y, x + size, y + size);
        var tl = (!hasTop && !hasLeft) ? radius : 0;
        var tr = (!hasTop && !hasRight) ? radius : 0;
        var br = (!hasBottom && !hasRight) ? radius : 0;
        var bl = (!hasBottom && !hasLeft) ? radius : 0;

        var roundRect = new SKRoundRect();
        roundRect.SetRectRadii(rect,
        [
            new SKPoint(tl, tl),
            new SKPoint(tr, tr),
            new SKPoint(br, br),
            new SKPoint(bl, bl)
        ]);

        canvas.DrawRoundRect(roundRect, paint);
    }

    private static void DrawFinderPattern(SKCanvas canvas, float x, float y, float moduleSize)
    {
        var outerSize = 7 * moduleSize;
        var middleSize = 5 * moduleSize;
        var innerSize = 3 * moduleSize;
        var outerRadius = moduleSize * 1.2f;
        var middleRadius = moduleSize * 0.8f;
        var innerRadius = moduleSize * 0.6f;

        using var darkPaint = new SKPaint { Color = new SKColor(33, 33, 33), IsAntialias = true, Style = SKPaintStyle.Fill };
        using var whitePaint = new SKPaint { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Fill };

        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x, y, x + outerSize, y + outerSize), outerRadius), darkPaint);

        var mo = moduleSize;
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x + mo, y + mo, x + mo + middleSize, y + mo + middleSize), middleRadius), whitePaint);

        var io = 2 * moduleSize;
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x + io, y + io, x + io + innerSize, y + io + innerSize), innerRadius), darkPaint);
    }
}