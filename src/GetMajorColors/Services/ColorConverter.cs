using System;
using System.Collections.Generic;
using System.Globalization;
using GetMajorColors.Models;
using SixLabors.ImageSharp.PixelFormats;

namespace GetMajorColors.Services;

/// <summary>
/// Converts RGB colors to other color models.
/// </summary>
public static class ColorConverter
{
    /// <summary>
    /// Converts an RGB pixel to values in the requested color model.
    /// </summary>
    public static IReadOnlyDictionary<string, double> ToModel(Rgb24 pixel, ColorModel model)
    {
        return model switch
        {
            ColorModel.Rgb => ToRgb(pixel),
            ColorModel.Cmyk => ToCmyk(pixel),
            ColorModel.Hsl => ToHsl(pixel),
            ColorModel.Hex => ToHex(pixel),
            _ => throw new NotSupportedException($"Color model '{model}' is not supported."),
        };
    }

    private static IReadOnlyDictionary<string, double> ToRgb(Rgb24 pixel)
    {
        return new Dictionary<string, double>
        {
            ["r"] = pixel.R,
            ["g"] = pixel.G,
            ["b"] = pixel.B,
        };
    }

    private static IReadOnlyDictionary<string, double> ToCmyk(Rgb24 pixel)
    {
        double r = pixel.R / 255.0;
        double g = pixel.G / 255.0;
        double b = pixel.B / 255.0;

        double k = 1.0 - Math.Max(r, Math.Max(g, b));
        double c = k == 1.0 ? 0.0 : (1.0 - r - k) / (1.0 - k);
        double m = k == 1.0 ? 0.0 : (1.0 - g - k) / (1.0 - k);
        double y = k == 1.0 ? 0.0 : (1.0 - b - k) / (1.0 - k);

        return new Dictionary<string, double>
        {
            ["c"] = Math.Round(c * 100.0, 2),
            ["m"] = Math.Round(m * 100.0, 2),
            ["y"] = Math.Round(y * 100.0, 2),
            ["k"] = Math.Round(k * 100.0, 2),
        };
    }

    private static IReadOnlyDictionary<string, double> ToHsl(Rgb24 pixel)
    {
        double r = pixel.R / 255.0;
        double g = pixel.G / 255.0;
        double b = pixel.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double delta = max - min;

        double h = 0.0;
        if (delta != 0.0)
        {
            if (max == r)
            {
                h = ((g - b) / delta + (g < b ? 6.0 : 0.0)) / 6.0;
            }
            else if (max == g)
            {
                h = ((b - r) / delta + 2.0) / 6.0;
            }
            else
            {
                h = ((r - g) / delta + 4.0) / 6.0;
            }
        }

        double l = (max + min) / 2.0;
        double s = delta == 0.0 ? 0.0 : delta / (1.0 - Math.Abs(2.0 * l - 1.0));

        return new Dictionary<string, double>
        {
            ["h"] = Math.Round(h * 360.0, 2),
            ["s"] = Math.Round(s, 4),
            ["l"] = Math.Round(l, 4),
        };
    }

    private static IReadOnlyDictionary<string, double> ToHex(Rgb24 pixel)
    {
        int hexValue = (pixel.R << 16) | (pixel.G << 8) | pixel.B;
        return new Dictionary<string, double>
        {
            ["hex"] = hexValue,
        };
    }

    /// <summary>
    /// Returns the hex string representation of an RGB pixel.
    /// </summary>
    public static string ToHexString(Rgb24 pixel)
    {
        return $"#{pixel.R:X2}{pixel.G:X2}{pixel.B:X2}";
    }
}
