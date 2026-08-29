using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;

namespace GetMajorColors.Services;

/// <summary>
/// Resolves an <see cref="ImageInput"/> into a readable image stream.
/// </summary>
public interface IImageSource
{
    /// <summary>
    /// Loads the image described by <paramref name="input"/> and returns a seekable stream.
    /// </summary>
    /// <param name="input">The image input descriptor.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that resolves to a seekable stream containing image bytes.</returns>
    Task<Stream> LoadAsync(ImageInput input, CancellationToken cancellationToken = default);
}
