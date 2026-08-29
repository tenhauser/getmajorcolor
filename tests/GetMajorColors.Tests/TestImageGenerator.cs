using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace GetMajorColors.Tests;

/// <summary>
/// Generates sample images for tests.
/// </summary>
public static class TestImageGenerator
{
    /// <summary>
    /// Creates a solid-color PNG image.
    /// </summary>
    public static byte[] CreateSolidColor(int width, int height, Rgb24 color)
    {
        using var image = new Image<Rgb24>(width, height, color);
        return SaveToBytes(image);
    }

    /// <summary>
    /// Creates a horizontal gradient between two colors.
    /// </summary>
    public static byte[] CreateHorizontalGradient(int width, int height, Rgb24 left, Rgb24 right)
    {
        using var image = new Image<Rgb24>(width, height);
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgb24> row = accessor.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                {
                    double t = x / (double)(accessor.Width - 1);
                    byte r = (byte)(left.R + (right.R - left.R) * t);
                    byte g = (byte)(left.G + (right.G - left.G) * t);
                    byte b = (byte)(left.B + (right.B - left.B) * t);
                    row[x] = new Rgb24(r, g, b);
                }
            }
        });

        return SaveToBytes(image);
    }

    private static byte[] SaveToBytes(Image<Rgb24> image)
    {
        var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return stream.ToArray();
    }
}
