using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;
using GetMajorColors.Services;
using YamlDotNet.Serialization;

namespace GetMajorColors.Output;

/// <summary>
/// Writes major colors as a YAML document.
/// </summary>
public sealed class YamlOutputWriter : IOutputWriter
{
    /// <inheritdoc />
    public async Task WriteAsync(TextWriter writer, IReadOnlyList<MajorColor> colors, ColorModel model, CancellationToken cancellationToken = default)
    {
        var outputColors = new List<OutputColor>(colors.Count);
        foreach (var color in colors)
        {
            outputColors.Add(new OutputColor(
                color.Name,
                color.Coverage,
                color.Values.Model.ToString().ToLowerInvariant(),
                color.Values.Components));
        }

        var document = new { colors = outputColors };
        var serializer = new SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        string yaml = serializer.Serialize(document);
        await writer.WriteAsync(yaml).ConfigureAwait(false);
    }
}
