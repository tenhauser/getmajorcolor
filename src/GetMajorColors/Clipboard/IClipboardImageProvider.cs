using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GetMajorColors.Clipboard;

/// <summary>
/// Reads image data from the system clipboard.
/// </summary>
public interface IClipboardImageProvider
{
    /// <summary>
    /// Attempts to read an image from the clipboard and returns it as a stream.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A seekable stream containing image bytes, or <c>null</c> if no image is available.</returns>
    Task<Stream?> GetImageAsync(CancellationToken cancellationToken = default);
}
