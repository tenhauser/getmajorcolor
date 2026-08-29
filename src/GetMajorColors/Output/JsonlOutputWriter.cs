using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;
using GetMajorColors.Services;

namespace GetMajorColors.Output;

/// <summary>
/// Writes one major color per line as JSON.
/// </summary>
public sealed class JsonlOutputWriter : IOutputWriter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <inheritdoc />
    public async Task WriteAsync(TextWriter writer, IReadOnlyList<MajorColor> colors, ColorModel model, CancellationToken cancellationToken = default)
    {
        foreach (var color in colors)
        {
            var output = new OutputColor(
                color.Name,
                color.Coverage,
                color.Values.Model.ToString().ToLowerInvariant(),
                color.Values.Components);
            string json = JsonSerializer.Serialize(output, Options);
            await writer.WriteLineAsync(json).ConfigureAwait(false);
        }
    }
}
