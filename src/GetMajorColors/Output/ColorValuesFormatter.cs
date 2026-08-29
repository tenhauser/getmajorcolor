using System;
using System.Globalization;
using System.Linq;
using GetMajorColors.Models;

namespace GetMajorColors.Output;

/// <summary>
/// Formats <see cref="ColorValues"/> as human-readable strings.
/// </summary>
public static class ColorValuesFormatter
{
    /// <summary>
    /// Formats the color values in a compact inline form suitable for text output.
    /// </summary>
    public static string Format(ColorValues values)
    {
        return values.Model switch
        {
            ColorModel.Rgb => $"rgb({values.Components["r"]:0}, {values.Components["g"]:0}, {values.Components["b"]:0})",
            ColorModel.Cmyk => $"cmyk({values.Components["c"]:0.##}, {values.Components["m"]:0.##}, {values.Components["y"]:0.##}, {values.Components["k"]:0.##})",
            ColorModel.Hsl => $"hsl({values.Components["h"]:0.##}, {values.Components["s"]:0.##}, {values.Components["l"]:0.##})",
            ColorModel.Hex => $"#{((int)values.Components["hex"]):X6}",
            _ => throw new NotSupportedException($"Color model '{values.Model}' is not supported."),
        };
    }

    /// <summary>
    /// Returns a hex string (#RRGGBB) from RGB component values.
    /// </summary>
    public static string ToHexString(ColorValues values)
    {
        if (values.Model == ColorModel.Hex)
        {
            return $"#{((int)values.Components["hex"]):X6}";
        }

        if (values.Components.TryGetValue("r", out double r) &&
            values.Components.TryGetValue("g", out double g) &&
            values.Components.TryGetValue("b", out double b))
        {
            return $"#{(byte)r:X2}{(byte)g:X2}{(byte)b:X2}";
        }

        throw new InvalidOperationException("Cannot produce hex string from non-RGB values.");
    }
}
