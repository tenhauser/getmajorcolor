using System.IO;
using System.Reflection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GetMajorColors.Tests;

/// <summary>
/// Ensures sample image files exist in the test output directory.
/// </summary>
public static class SampleImages
{
    /// <summary>
    /// Directory containing sample test images.
    /// </summary>
    public static string DirectoryPath { get; } = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "TestImages");

    /// <summary>
    /// Paths to generated sample images.
    /// </summary>
    public static class Paths
    {
        public static string SolidRed => Path.Combine(DirectoryPath, "solid-red-100x100.png");
        public static string GradientRgb => Path.Combine(DirectoryPath, "gradient-rgb-200x200.png");
    }

    /// <summary>
    /// Creates sample images if they do not already exist.
    /// </summary>
    public static void EnsureExists()
    {
        if (!Directory.Exists(DirectoryPath))
        {
            Directory.CreateDirectory(DirectoryPath);
        }

        if (!File.Exists(Paths.SolidRed))
        {
            using var image = new Image<Rgb24>(100, 100, new Rgb24(255, 0, 0));
            image.SaveAsPng(Paths.SolidRed);
        }

        if (!File.Exists(Paths.GradientRgb))
        {
            using var image = new Image<Rgb24>(200, 200);
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        double t = x / (double)(accessor.Width - 1);
                        byte r = (byte)(255 * t);
                        byte g = (byte)(255 * (1 - t));
                        byte b = 0;
                        row[x] = new Rgb24(r, g, b);
                    }
                }
            });

            image.SaveAsPng(Paths.GradientRgb);
        }
    }
}
