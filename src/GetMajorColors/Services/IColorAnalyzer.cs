using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;

namespace GetMajorColors.Services;

/// <summary>
/// Analyzes an image stream and returns the major colors.
/// </summary>
public interface IColorAnalyzer
{
    /// <summary>
    /// Identifies the major colors in the image stream.
    /// </summary>
    /// <param name="imageStream">Seekable stream containing the image.</param>
    /// <param name="count">Number of major colors to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The major colors sorted by coverage descending.</returns>
    Task<IReadOnlyList<MajorColor>> AnalyzeAsync(Stream imageStream, int count, CancellationToken cancellationToken = default);
}
