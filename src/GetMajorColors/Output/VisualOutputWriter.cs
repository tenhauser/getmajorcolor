using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;
using GetMajorColors.Services;

namespace GetMajorColors.Output;

/// <summary>
/// Writes major colors as ANSI truecolor swatches.
/// </summary>
public sealed class VisualOutputWriter : IOutputWriter
{
    /// <inheritdoc />
    public Task WriteAsync(TextWriter writer, IReadOnlyList<MajorColor> colors, ColorModel model, CancellationToken cancellationToken = default)
    {
        if (!SupportsAnsiTrueColor())
        {
            throw new InvalidOperationException("Terminal does not support ANSI truecolor output.");
        }

        foreach (var color in colors)
        {
            string hex = ColorValuesFormatter.ToHexString(color.Values);
            (byte r, byte g, byte b) = ToRgbBytes(color.Values);
            string swatch = $"\x1b[48;2;{r};{g};{b}m  \x1b[0m";
            writer.WriteLine($"{swatch} {color.Name} {hex} {color.Coverage * 100.0:0.0}%");
        }

        return Task.CompletedTask;
    }

    private static bool SupportsAnsiTrueColor()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NO_COLOR")))
        {
            return false;
        }

        string? term = Environment.GetEnvironmentVariable("TERM");
        string? colorterm = Environment.GetEnvironmentVariable("COLORTERM");

        return !string.IsNullOrEmpty(colorterm) ||
               (!string.IsNullOrEmpty(term) && (term.Contains("256color") || term.Contains("truecolor")));
    }

    private static (byte R, byte G, byte B) ToRgbBytes(ColorValues values)
    {
        if (values.Components.TryGetValue("r", out double r) &&
            values.Components.TryGetValue("g", out double g) &&
            values.Components.TryGetValue("b", out double b))
        {
            return ((byte)r, (byte)g, (byte)b);
        }

        throw new InvalidOperationException("Visual output requires RGB component values.");
    }
}
