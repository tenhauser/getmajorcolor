using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;

namespace GetMajorColors.Services;

/// <summary>
/// Formats a collection of major colors and writes them to a text writer.
/// </summary>
public interface IOutputWriter
{
    /// <summary>
    /// Writes the formatted results to <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The destination text writer.</param>
    /// <param name="colors">The major colors to output.</param>
    /// <param name="model">The color model used for the numeric values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task WriteAsync(TextWriter writer, IReadOnlyList<MajorColor> colors, ColorModel model, CancellationToken cancellationToken = default);
}
