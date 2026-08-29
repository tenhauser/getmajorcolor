using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;
using GetMajorColors.Services;

namespace GetMajorColors.Output;

/// <summary>
/// Writes major colors as a single JSON object.
/// </summary>
public sealed class JsonOutputWriter : IOutputWriter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    /// <inheritdoc />
    public async Task WriteAsync(TextWriter writer, IReadOnlyList<MajorColor> colors, ColorModel model, CancellationToken cancellationToken = default)
    {
        var document = new
        {
            colors = ToOutputColors(colors),
        };

        string json = JsonSerializer.Serialize(document, Options);
        await writer.WriteAsync(json).ConfigureAwait(false);
        await writer.WriteLineAsync().ConfigureAwait(false);
    }

    private static List<OutputColor> ToOutputColors(IReadOnlyList<MajorColor> colors)
    {
        var output = new List<OutputColor>(colors.Count);
        foreach (var color in colors)
        {
            output.Add(new OutputColor(
                color.Name,
                color.Coverage,
                color.Values.Model.ToString().ToLowerInvariant(),
                color.Values.Components));
        }

        return output;
    }
}
