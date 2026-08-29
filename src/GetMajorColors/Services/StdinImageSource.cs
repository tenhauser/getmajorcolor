using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;

namespace GetMajorColors.Services;

/// <summary>
/// Loads an image from standard input.
/// </summary>
public sealed class StdinImageSource : IImageSource
{
    private const long MaxInputBytes = 100 * 1024 * 1024;

    /// <inheritdoc />
    public async Task<Stream> LoadAsync(ImageInput input, CancellationToken cancellationToken = default)
    {
        if (input.SourceKind != ImageSourceKind.StandardInput)
        {
            throw new ArgumentException("Expected a standard input source.", nameof(input));
        }

        if (input.Stream is null)
        {
            throw new ArgumentException("Standard input stream is missing.", nameof(input));
        }

        var memoryStream = new MemoryStream();
        await input.Stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);

        if (memoryStream.Length > MaxInputBytes)
        {
            throw new InvalidOperationException($"Input exceeds maximum size of {MaxInputBytes / (1024 * 1024)} MB.");
        }

        if (memoryStream.Length == 0)
        {
            throw new InvalidOperationException("Standard input is empty.");
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
