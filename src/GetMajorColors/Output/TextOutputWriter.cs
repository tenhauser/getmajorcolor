using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;
using GetMajorColors.Services;

namespace GetMajorColors.Output;

/// <summary>
/// Writes major colors as human-readable text lines.
/// </summary>
public sealed class TextOutputWriter : IOutputWriter
{
    /// <inheritdoc />
    public Task WriteAsync(TextWriter writer, IReadOnlyList<MajorColor> colors, ColorModel model, CancellationToken cancellationToken = default)
    {
        foreach (var color in colors)
        {
            string values = ColorValuesFormatter.Format(color.Values);
            string hexPrefix = color.Values.Model == ColorModel.Rgb
                ? ColorValuesFormatter.ToHexString(color.Values) + " "
                : string.Empty;
            writer.WriteLine($"{color.Name} {hexPrefix}{values} {colorCoverageText(color.Coverage)}");
        }

        return Task.CompletedTask;
    }

    private static string colorCoverageText(double coverage)
    {
        return $"{coverage * 100.0:0.0}%";
    }
}
