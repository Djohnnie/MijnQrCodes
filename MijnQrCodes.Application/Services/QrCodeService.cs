using System.Collections;
using System.Text;
using QRCoder;
using SkiaSharp;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace MijnQrCodes.Application.Services;

public class QrCodeService : IQrCodeService
{
    private const int QuietZoneModules = 2;

    public byte[] GenerateQrCode(string content, int size = 1024,
        string backgroundColor = "#FFFFFF", string foregroundColor = "#212121",
        string finderPatternColor = "#212121", byte[]? centerImageData = null)
    {
        var bgColor = SKColor.Parse(backgroundColor);
        var fgColor = SKColor.Parse(foregroundColor);
        var fpColor = SKColor.Parse(finderPatternColor);

        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
        var moduleData = qrCodeData.ModuleMatrix;
        var moduleCount = moduleData.Count;

        var totalModules = moduleCount + QuietZoneModules * 2;
        var moduleSize = (float)size / totalModules;
        var cornerRadius = moduleSize * 0.4f;

        using var surface = SKSurface.Create(new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul));
        var canvas = surface.Canvas;
        canvas.Clear(bgColor);

        using var fgPaint = new SKPaint
        {
            Color = fgColor,
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
                    hasTop, hasRight, hasBottom, hasLeft, fgPaint);
            }
        }

        DrawFinderPattern(canvas, QuietZoneModules * moduleSize, QuietZoneModules * moduleSize, moduleSize, fpColor, bgColor);
        DrawFinderPattern(canvas, (moduleCount - 7 + QuietZoneModules) * moduleSize, QuietZoneModules * moduleSize, moduleSize, fpColor, bgColor);
        DrawFinderPattern(canvas, QuietZoneModules * moduleSize, (moduleCount - 7 + QuietZoneModules) * moduleSize, moduleSize, fpColor, bgColor);

        if (centerImageData is { Length: > 0 })
        {
            DrawCenterImage(canvas, centerImageData, size, moduleCount, totalModules, moduleSize, bgColor);
        }

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

    private static void DrawFinderPattern(SKCanvas canvas, float x, float y, float moduleSize,
        SKColor finderColor, SKColor backgroundColor)
    {
        var outerSize = 7 * moduleSize;
        var middleSize = 5 * moduleSize;
        var innerSize = 3 * moduleSize;
        var outerRadius = moduleSize * 1.2f;
        var middleRadius = moduleSize * 0.8f;
        var innerRadius = moduleSize * 0.6f;

        using var fpPaint = new SKPaint { Color = finderColor, IsAntialias = true, Style = SKPaintStyle.Fill };
        using var bgPaint = new SKPaint { Color = backgroundColor, IsAntialias = true, Style = SKPaintStyle.Fill, BlendMode = SKBlendMode.Src };

        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x, y, x + outerSize, y + outerSize), outerRadius), fpPaint);

        var mo = moduleSize;
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x + mo, y + mo, x + mo + middleSize, y + mo + middleSize), middleRadius), bgPaint);

        var io = 2 * moduleSize;
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x + io, y + io, x + io + innerSize, y + io + innerSize), innerRadius), fpPaint);
    }

    private static void DrawCenterImage(SKCanvas canvas, byte[] imageData, int size,
        int moduleCount, int totalModules, float moduleSize, SKColor bgColor)
    {
        var centerModules = (int)Math.Ceiling(moduleCount * 0.25);
        if (centerModules % 2 != moduleCount % 2) centerModules++;
        var centerPixelSize = centerModules * moduleSize;

        var centerX = (size - centerPixelSize) / 2f;
        var centerY = (size - centerPixelSize) / 2f;
        var padding = moduleSize;
        var bgRect = new SKRect(
            centerX - padding, centerY - padding,
            centerX + centerPixelSize + padding, centerY + centerPixelSize + padding);

        using var bgPaint = new SKPaint
        {
            Color = bgColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Src
        };
        var bgRadius = moduleSize * 1.2f;
        canvas.DrawRoundRect(new SKRoundRect(bgRect, bgRadius), bgPaint);

        var destRect = new SKRect(centerX, centerY, centerX + centerPixelSize, centerY + centerPixelSize);

        if (IsSvg(imageData))
        {
            var svg = new SKSvg();
            using var stream = new MemoryStream(imageData);
            svg.Load(stream);
            if (svg.Picture != null)
            {
                var svgBounds = svg.Picture.CullRect;
                var scaleX = centerPixelSize / svgBounds.Width;
                var scaleY = centerPixelSize / svgBounds.Height;
                var scale = Math.Min(scaleX, scaleY);

                canvas.Save();
                canvas.Translate(centerX + (centerPixelSize - svgBounds.Width * scale) / 2f,
                                 centerY + (centerPixelSize - svgBounds.Height * scale) / 2f);
                canvas.Scale(scale);
                canvas.DrawPicture(svg.Picture);
                canvas.Restore();
            }
        }
        else
        {
            using var bitmap = SKBitmap.Decode(imageData);
            if (bitmap == null) return;
            using var imgPaint = new SKPaint { IsAntialias = true, FilterQuality = SKFilterQuality.High };
            canvas.DrawBitmap(bitmap, destRect, imgPaint);
        }
    }

    private static bool IsSvg(byte[] data)
    {
        if (data.Length < 5) return false;
        var header = Encoding.UTF8.GetString(data, 0, Math.Min(data.Length, 256));
        return header.Contains("<svg", StringComparison.OrdinalIgnoreCase);
    }
}