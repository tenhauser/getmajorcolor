using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;
using GetMajorColors.Services;

namespace GetMajorColors.Clipboard;

/// <summary>
/// Loads an image from the system clipboard via a platform-specific provider.
/// </summary>
public sealed class ClipboardImageSource : IImageSource
{
    private readonly IClipboardImageProvider _provider;

    /// <summary>
    /// Creates a new clipboard image source.
    /// </summary>
    public ClipboardImageSource(IClipboardImageProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public async Task<Stream> LoadAsync(ImageInput input, CancellationToken cancellationToken = default)
    {
        if (input.SourceKind != ImageSourceKind.Clipboard)
        {
            throw new ArgumentException("Expected a clipboard input.", nameof(input));
        }

        Stream? stream = await _provider.GetImageAsync(cancellationToken).ConfigureAwait(false);
        if (stream is null)
        {
            throw new InvalidOperationException("Clipboard does not contain image data.");
        }

        return stream;
    }
}
