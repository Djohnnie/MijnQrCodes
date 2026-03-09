using System.Collections;
using System.Text;
using QRCoder;
using SkiaSharp;
using Svg.Skia;

namespace MijnQrCodes.Application.Services;

/// <summary>
/// Generates stylized QR codes with rounded modules, custom finder patterns, and optional center images.
/// Uses SkiaSharp for rendering and QRCoder for QR code data generation.
/// The output is a PNG-encoded byte array suitable for display or storage.
/// </summary>
/// <remarks>
/// <para>
/// The QR code is generated with <see cref="QRCodeGenerator.ECCLevel.H"/> (high error correction),
/// which allows up to 30% of the code to be obscured (e.g., by a center image) while remaining scannable.
/// </para>
/// <para>
/// The rendering pipeline:
/// <list type="number">
///   <item>Generate raw QR module matrix using QRCoder.</item>
///   <item>Draw rounded data modules with adaptive corner radii based on neighbors.</item>
///   <item>Draw inner corner arcs for diagonal neighbor connections to create smooth visual flow.</item>
///   <item>Overlay custom rounded finder patterns (the three large squares) on top.</item>
///   <item>Optionally place a center image (SVG or raster) with a background padding area.</item>
/// </list>
/// </para>
/// </remarks>
public class QrCodeService : IQrCodeService
{
    /// <summary>
    /// Generates a stylized QR code image as a PNG byte array.
    /// </summary>
    /// <param name="content">
    /// The data to encode in the QR code (e.g., a URL, plain text, or vCard).
    /// Longer content increases the number of modules and may reduce scannability at small sizes.
    /// </param>
    /// <param name="size">
    /// The width and height of the output image in pixels. Defaults to 1024.
    /// The image is always square. Larger values produce sharper output.
    /// </param>
    /// <param name="backgroundColor">
    /// The background color of the QR code as a hex string (e.g., "#FFFFFF" for white).
    /// Must contrast sufficiently with <paramref name="foregroundColor"/> for reliable scanning.
    /// </param>
    /// <param name="foregroundColor">
    /// The color of the data modules (the small squares) as a hex string (e.g., "#212121" for near-black).
    /// </param>
    /// <param name="finderPatternColor">
    /// The color of the three finder patterns (large corner squares) as a hex string.
    /// Can differ from <paramref name="foregroundColor"/> to create a two-tone visual style.
    /// </param>
    /// <param name="centerImageData">
    /// Optional raw image bytes (PNG, JPEG, or SVG) to overlay at the center of the QR code.
    /// The image occupies approximately 25% of the QR code area. High error correction (level H)
    /// ensures the code remains scannable despite the obscured center region.
    /// </param>
    /// <param name="centerImageColor">
    /// Optional hex color string to tint the center image. Only applies to SVG images,
    /// where it replaces all colors using a <see cref="SKBlendMode.SrcIn"/> color filter.
    /// Ignored for raster images.
    /// </param>
    /// <returns>A PNG-encoded byte array representing the rendered QR code image.</returns>
    public byte[] GenerateQrCode(string content, int size = 4096,
        string backgroundColor = "#FFFFFF", string foregroundColor = "#212121",
        string finderPatternColor = "#212121", byte[]? centerImageData = null,
        string? centerImageColor = null)
    {
        // Parse hex color strings into SkiaSharp color structs.
        var bgColor = SKColor.Parse(backgroundColor);
        var fgColor = SKColor.Parse(foregroundColor);
        var fpColor = SKColor.Parse(finderPatternColor);

        // Generate the raw QR code data with high error correction (H = ~30% recovery capacity),
        // which is essential when a center image will obscure part of the code.
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
        var moduleData = qrCodeData.ModuleMatrix;
        var moduleCount = moduleData.Count;

        // When a center image is present, blank out the modules that will be covered by it
        // (the image area plus its one-module padding), so no rounded corners bleed through.
        if (centerImageData is { Length: > 0 })
        {
            var centerModules = (int)Math.Ceiling(moduleCount * 0.25);
            if (centerModules % 2 != moduleCount % 2) centerModules++;
            var centerStart = (moduleCount - centerModules) / 2 - 1;
            var centerEnd = centerStart + centerModules + 2;
            for (var row = Math.Max(0, centerStart); row < Math.Min(moduleCount, centerEnd); row++)
                for (var col = Math.Max(0, centerStart); col < Math.Min(moduleCount, centerEnd); col++)
                    moduleData[row][col] = false;
        }

        // Calculate layout dimensions:
        // - moduleSize is the pixel size of each individual module (square cell).
        // - cornerRadius controls the roundness of each module (40% of module size).
        var moduleSize = (float)size / moduleCount;
        var cornerRadius = moduleSize * 0.4f;

        // Create an off-screen SkiaSharp surface with premultiplied alpha for proper blending.
        using var surface = SKSurface.Create(new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul));
        var canvas = surface.Canvas;
        canvas.Clear(bgColor);

        // Shared paint object for all data modules (reused for performance).
        using var fgPaint = new SKPaint
        {
            Color = fgColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        // Iterate over every module in the QR matrix and draw data modules with rounded corners.
        // Finder pattern areas are skipped here because they are drawn separately with a custom style.
        for (var row = 0; row < moduleCount; row++)
        {
            for (var col = 0; col < moduleCount; col++)
            {
                // Skip empty modules (white cells) and finder pattern regions.
                if (!moduleData[row][col]) continue;
                if (IsFinderPatternArea(row, col, moduleCount)) continue;

                // Convert module grid coordinates to pixel coordinates, offset by the quiet zone.
                var x = col * moduleSize;
                var y = row * moduleSize;

                // Check which orthogonal neighbors are active data modules.
                // This determines which corners of this module should be rounded:
                // a corner is rounded only when both adjacent edges are free (no neighbor).
                var hasTop = IsDataModule(moduleData, row - 1, col, moduleCount);
                var hasBottom = IsDataModule(moduleData, row + 1, col, moduleCount);
                var hasLeft = IsDataModule(moduleData, row, col - 1, moduleCount);
                var hasRight = IsDataModule(moduleData, row, col + 1, moduleCount);

                DrawRoundedModule(canvas, x, y, moduleSize, cornerRadius,
                    hasTop, hasRight, hasBottom, hasLeft, fgPaint);

                // Draw inner corner arcs for diagonal neighbors.
                // When two modules are diagonally adjacent but share no orthogonal neighbor,
                // a small arc is drawn in the gap to create a smooth, connected visual appearance
                // instead of leaving a distracting notch between the rounded corners.
                if (hasTop && hasLeft && !IsDataModule(moduleData, row - 1, col - 1, moduleCount))
                    DrawInnerCorner(canvas, x, y, cornerRadius, 180f, fgPaint);
                if (hasTop && hasRight && !IsDataModule(moduleData, row - 1, col + 1, moduleCount))
                    DrawInnerCorner(canvas, x + moduleSize, y, cornerRadius, 270f, fgPaint);
                if (hasBottom && hasRight && !IsDataModule(moduleData, row + 1, col + 1, moduleCount))
                    DrawInnerCorner(canvas, x + moduleSize, y + moduleSize, cornerRadius, 0f, fgPaint);
                if (hasBottom && hasLeft && !IsDataModule(moduleData, row + 1, col - 1, moduleCount))
                    DrawInnerCorner(canvas, x, y + moduleSize, cornerRadius, 90f, fgPaint);
            }
        }

        // Draw the three finder patterns (7×7 concentric rounded squares) at fixed positions:
        // top-left, top-right, and bottom-left corners of the QR code.
        // These patterns enable scanners to detect the code's orientation and boundaries.
        DrawFinderPattern(canvas, 4 * moduleSize, 4 * moduleSize, moduleSize, fpColor, bgColor);
        DrawFinderPattern(canvas, (moduleCount - 11) * moduleSize, 4 * moduleSize, moduleSize, fpColor, bgColor);
        DrawFinderPattern(canvas, 4 * moduleSize, (moduleCount - 11) * moduleSize, moduleSize, fpColor, bgColor);

        // Optionally render a center image (logo/icon) over the QR code.
        // The high error correction level allows this region to be obscured.
        if (centerImageData is { Length: > 0 })
        {
            DrawCenterImage(canvas, centerImageData, size, moduleCount, moduleSize, bgColor, centerImageColor);
        }

        // Encode the rendered surface as a PNG image and return the byte array.
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    /// <summary>
    /// Determines whether a module at the given position is an active data module.
    /// A data module is one that is "on" (dark) and is NOT part of a finder pattern area.
    /// </summary>
    /// <param name="moduleData">The QR code module matrix (rows of bit arrays).</param>
    /// <param name="row">The row index to check (may be out of bounds).</param>
    /// <param name="col">The column index to check (may be out of bounds).</param>
    /// <param name="moduleCount">The total number of modules per row/column.</param>
    /// <returns>
    /// <c>true</c> if the position is within bounds, is not a finder pattern area,
    /// and the module is active (dark); otherwise <c>false</c>.
    /// </returns>
    private static bool IsDataModule(List<BitArray> moduleData, int row, int col, int moduleCount)
    {
        if (row < 0 || row >= moduleCount || col < 0 || col >= moduleCount) return false;
        if (IsFinderPatternArea(row, col, moduleCount)) return false;
        return moduleData[row][col];
    }

    /// <summary>
    /// Checks whether the given module position falls within one of the three finder pattern areas.
    /// Finder patterns are 7×7 module regions located at three corners of the QR code:
    /// top-left, top-right, and bottom-left. The bottom-right corner is intentionally left empty.
    /// </summary>
    /// <param name="row">The row index in the module matrix.</param>
    /// <param name="col">The column index in the module matrix.</param>
    /// <param name="moduleCount">The total number of modules per row/column.</param>
    /// <returns><c>true</c> if the position is within a finder pattern region; otherwise <c>false</c>.</returns>
    private static bool IsFinderPatternArea(int row, int col, int moduleCount)
    {
        // Top-left finder pattern: rows 0–6, columns 0–6.
        if (row < 7 && col < 7) return true;
        // Top-right finder pattern: rows 0–6, last 7 columns.
        if (row < 7 && col >= moduleCount - 7) return true;
        // Bottom-left finder pattern: last 7 rows, columns 0–6.
        if (row >= moduleCount - 7 && col < 7) return true;
        return false;
    }

    /// <summary>
    /// Draws a single QR data module as a rounded rectangle with individually controlled corner radii.
    /// Each corner is rounded only when the module has no orthogonal neighbor on both adjacent sides,
    /// creating a smooth, pill-like visual when modules are grouped together.
    /// </summary>
    /// <param name="canvas">The SkiaSharp canvas to draw on.</param>
    /// <param name="x">The x-coordinate (in pixels) of the module's top-left corner.</param>
    /// <param name="y">The y-coordinate (in pixels) of the module's top-left corner.</param>
    /// <param name="size">The width and height of the module in pixels.</param>
    /// <param name="radius">The corner radius to apply when a corner is rounded.</param>
    /// <param name="hasTop">Whether an adjacent data module exists directly above.</param>
    /// <param name="hasRight">Whether an adjacent data module exists directly to the right.</param>
    /// <param name="hasBottom">Whether an adjacent data module exists directly below.</param>
    /// <param name="hasLeft">Whether an adjacent data module exists directly to the left.</param>
    /// <param name="paint">The paint (color, style) to use for filling the module.</param>
    private static void DrawRoundedModule(SKCanvas canvas, float x, float y, float size, float radius,
        bool hasTop, bool hasRight, bool hasBottom, bool hasLeft, SKPaint paint)
    {
        var rect = new SKRect(x, y, x + size, y + size);

        // A corner is rounded (uses radius) only when neither adjacent edge has a neighbor.
        // This creates seamless joins between connected modules.
        var tl = (!hasTop && !hasLeft) ? radius : 0;
        var tr = (!hasTop && !hasRight) ? radius : 0;
        var br = (!hasBottom && !hasRight) ? radius : 0;
        var bl = (!hasBottom && !hasLeft) ? radius : 0;

        // SetRectRadii accepts four SKPoint values, one per corner (TL, TR, BR, BL),
        // where each point's X and Y define the horizontal and vertical radius.
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

    /// <summary>
    /// Draws a small 90-degree arc (inner corner fill) at a module corner to visually connect
    /// two diagonally adjacent modules that do not share an orthogonal neighbor.
    /// Without this, rounded modules that are only diagonally adjacent would have a visible
    /// notch/gap between them, which looks unpolished.
    /// </summary>
    /// <param name="canvas">The SkiaSharp canvas to draw on.</param>
    /// <param name="cx">The x-coordinate of the corner point where the arc is anchored.</param>
    /// <param name="cy">The y-coordinate of the corner point where the arc is anchored.</param>
    /// <param name="radius">The radius of the arc, matching the module corner radius.</param>
    /// <param name="startAngle">
    /// The starting angle of the arc in degrees. Determines which diagonal direction to fill:
    /// <list type="bullet">
    ///   <item><description>0° — bottom-right diagonal.</description></item>
    ///   <item><description>90° — bottom-left diagonal.</description></item>
    ///   <item><description>180° — top-left diagonal.</description></item>
    ///   <item><description>270° — top-right diagonal.</description></item>
    /// </list>
    /// </param>
    /// <param name="paint">The paint (color, style) to use for filling the arc.</param>
    /// <remarks>
    /// The arc is drawn as a concave shape: the circle driving the arc is centered diagonally
    /// away from <paramref name="cx"/>/<paramref name="cy"/>, so the arc bows <em>toward</em>
    /// the corner rather than away from it, producing the correct inside-corner fill.
    /// </remarks>
    private static void DrawInnerCorner(SKCanvas canvas, float cx, float cy, float radius,
        float startAngle, SKPaint paint)
    {
        // Build a concave arc path: the circle center is offset diagonally from the corner,
        // and the startAngle is rotated 180° so the arc curves inward toward (cx, cy).
        var (ocx, ocy) = startAngle switch
        {
            180f => (cx - radius, cy - radius),
            270f => (cx + radius, cy - radius),
            0f   => (cx + radius, cy + radius),
            _    => (cx - radius, cy + radius)
        };
        using var path = new SKPath();
        path.MoveTo(cx, cy);
        var oval = new SKRect(ocx - radius, ocy - radius, ocx + radius, ocy + radius);
        path.ArcTo(oval, (startAngle + 180f) % 360f, 90f, false);
        path.Close();
        canvas.DrawPath(path, paint);
    }

    /// <summary>
    /// Draws a single finder pattern at the specified position. Finder patterns are the three
    /// large concentric squares located at the top-left, top-right, and bottom-left corners
    /// of every QR code. Scanners use these to determine the code's position and orientation.
    /// </summary>
    /// <remarks>
    /// The pattern consists of three nested rounded rectangles:
    /// <list type="bullet">
    ///   <item><description>Outer ring: 7×7 modules, drawn in the finder color.</description></item>
    ///   <item><description>Middle gap: 5×5 modules, drawn in the background color (erases the center of the outer ring).</description></item>
    ///   <item><description>Inner dot: 3×3 modules, drawn in the finder color.</description></item>
    /// </list>
    /// Each layer uses a progressively smaller corner radius for visual harmony.
    /// The middle layer uses <see cref="SKBlendMode.Src"/> to fully replace pixels (including alpha),
    /// ensuring a clean cutout even on non-white backgrounds.
    /// </remarks>
    /// <param name="canvas">The SkiaSharp canvas to draw on.</param>
    /// <param name="x">The x-coordinate (in pixels) of the finder pattern's top-left corner.</param>
    /// <param name="y">The y-coordinate (in pixels) of the finder pattern's top-left corner.</param>
    /// <param name="moduleSize">The pixel size of one module, used to calculate pattern dimensions.</param>
    /// <param name="finderColor">The color for the outer ring and inner dot.</param>
    /// <param name="backgroundColor">The color for the middle gap layer.</param>
    private static void DrawFinderPattern(SKCanvas canvas, float x, float y, float moduleSize,
        SKColor finderColor, SKColor backgroundColor)
    {
        // Calculate the pixel dimensions for each concentric layer.
        var outerSize = 7 * moduleSize;
        var middleSize = 5 * moduleSize;
        var innerSize = 3 * moduleSize;

        // Progressively smaller corner radii create a visually balanced nested appearance.
        var outerRadius = moduleSize * 1.2f;
        var middleRadius = moduleSize * 0.8f;
        var innerRadius = moduleSize * 0.6f;

        using var fpPaint = new SKPaint { Color = finderColor, IsAntialias = true, Style = SKPaintStyle.Fill };
        // BlendMode.Src ensures the background paint fully replaces the underlying pixels,
        // which is necessary to cleanly "punch out" the middle ring from the outer square.
        using var bgPaint = new SKPaint { Color = backgroundColor, IsAntialias = true, Style = SKPaintStyle.Fill, BlendMode = SKBlendMode.Src };

        // Layer 0: Background
        canvas.DrawRect(new SKRect(x, y, x + outerSize, y + outerSize), bgPaint);

        // Layer 1: Outer ring (7×7 modules).
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x, y, x + outerSize, y + outerSize), outerRadius), fpPaint);

        // Layer 2: Middle gap (5×5 modules), inset by 1 module on each side.
        var mo = moduleSize;
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x + mo, y + mo, x + mo + middleSize, y + mo + middleSize), middleRadius), bgPaint);

        // Layer 3: Inner dot (3×3 modules), inset by 2 modules on each side.
        var io = 2 * moduleSize;
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(x + io, y + io, x + io + innerSize, y + io + innerSize), innerRadius), fpPaint);
    }

    /// <summary>
    /// Draws an image (logo or icon) at the center of the QR code. The image is placed over
    /// a background-colored padding area that clears the underlying QR modules, relying on
    /// the high error correction level (H) to maintain scannability.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The center area spans approximately 25% of the module count (adjusted for parity alignment),
    /// plus one module of padding on each side. This ensures the image doesn't overlap with
    /// critical QR data beyond what error correction can recover.
    /// </para>
    /// <para>
    /// SVG images are rendered as vector graphics using <see cref="SKSvg"/> and can optionally
    /// be tinted to a single color via <paramref name="colorOverride"/>. Raster images (PNG, JPEG)
    /// are decoded as bitmaps and scaled to fit the center area with high-quality filtering.
    /// </para>
    /// </remarks>
    /// <param name="canvas">The SkiaSharp canvas to draw on.</param>
    /// <param name="imageData">Raw image bytes (SVG, PNG, or JPEG).</param>
    /// <param name="size">The total QR code image size in pixels.</param>
    /// <param name="moduleCount">The number of modules per row/column in the QR matrix.</param>
    /// <param name="totalModules">The total modules including quiet zone (<paramref name="moduleCount"/> + 2 × quiet zone).</param>
    /// <param name="moduleSize">The pixel size of one module.</param>
    /// <param name="bgColor">The background color used to clear the area behind the center image.</param>
    /// <param name="colorOverride">
    /// Optional hex color string to tint SVG images. When provided, all SVG colors are replaced
    /// using a <see cref="SKBlendMode.SrcIn"/> filter. Has no effect on raster images.
    /// </param>
    private static void DrawCenterImage(SKCanvas canvas, byte[] imageData, int size,
        int moduleCount, float moduleSize, SKColor bgColor, string? colorOverride = null)
    {
        // Calculate the center area size in modules (~25% of the QR code).
        // Adjust parity so the center area aligns symmetrically with the module grid.
        var centerModules = (int)Math.Ceiling(moduleCount * 0.25);
        if (centerModules % 2 != moduleCount % 2) centerModules++;
        var centerPixelSize = centerModules * moduleSize;

        // Position the center area exactly in the middle of the QR code.
        var centerX = (size - centerPixelSize) / 2f;
        var centerY = (size - centerPixelSize) / 2f;

        // Add one module of padding around the image to create a clean border
        // between the image and the surrounding QR data modules.
        var padding = moduleSize;
        var bgRect = new SKRect(
            centerX - padding, centerY - padding,
            centerX + centerPixelSize + padding, centerY + centerPixelSize + padding);

        // Clear the center area with the background color using Src blend mode
        // to fully replace all underlying pixel data (including any drawn modules).
        using var bgPaint = new SKPaint
        {
            Color = bgColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Src
        };
        var bgRadius = moduleSize * 1.2f;
        canvas.DrawRoundRect(new SKRoundRect(bgRect, bgRadius), bgPaint);

        // The destination rectangle for the image (without padding).
        var destRect = new SKRect(centerX, centerY, centerX + centerPixelSize, centerY + centerPixelSize);

        if (IsSvg(imageData))
        {
            // SVG rendering path: load as a vector picture and scale to fit.
            using var svg = new SKSvg();
            using var stream = new MemoryStream(imageData);
            svg.Load(stream);
            if (svg.Picture != null)
            {
                // Calculate uniform scale factor to fit the SVG within the center area
                // while preserving its aspect ratio (uses the smaller of width/height scales).
                var svgBounds = svg.Picture.CullRect;
                var scaleX = centerPixelSize / svgBounds.Width;
                var scaleY = centerPixelSize / svgBounds.Height;
                var scale = Math.Min(scaleX, scaleY);

                // Apply canvas transformations: translate to center the scaled image,
                // then scale to the target size. Save/Restore isolates these transforms.
                canvas.Save();
                canvas.Translate(centerX + (centerPixelSize - svgBounds.Width * scale) / 2f,
                                 centerY + (centerPixelSize - svgBounds.Height * scale) / 2f);
                canvas.Scale(scale);

                if (colorOverride is not null)
                {
                    // Apply a color tint using SrcIn blend mode: this replaces all visible
                    // (non-transparent) pixels with the override color while preserving alpha.
                    using var colorPaint = new SKPaint
                    {
                        ColorFilter = SKColorFilter.CreateBlendMode(
                            SKColor.Parse(colorOverride), SKBlendMode.SrcIn),
                        IsAntialias = true
                    };
                    canvas.DrawPicture(svg.Picture, colorPaint);
                }
                else
                {
                    canvas.DrawPicture(svg.Picture);
                }

                canvas.Restore();
            }
        }
        else
        {
            // Raster image rendering path: decode as a bitmap and draw scaled to fit.
            using var bitmap = SKBitmap.Decode(imageData);
            if (bitmap == null) return;
            using var imgPaint = new SKPaint { IsAntialias = true };
            canvas.DrawBitmap(bitmap, destRect, imgPaint);
        }
    }

    /// <summary>
    /// Detects whether the given byte array contains SVG content by inspecting the first
    /// 256 bytes for an <c>&lt;svg</c> tag. This is a heuristic check (not a full XML parse)
    /// and is used to choose between the SVG and raster rendering paths in
    /// <see cref="DrawCenterImage"/>.
    /// </summary>
    /// <param name="data">The raw image bytes to inspect.</param>
    /// <returns><c>true</c> if the data appears to be an SVG document; otherwise <c>false</c>.</returns>
    private static bool IsSvg(byte[] data)
    {
        if (data.Length < 5) return false;
        var header = Encoding.UTF8.GetString(data, 0, Math.Min(data.Length, 256));
        return header.Contains("<svg", StringComparison.OrdinalIgnoreCase);
    }
}