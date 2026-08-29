using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;

namespace GetMajorColors.Services;

/// <summary>
/// Loads an image from a file path.
/// </summary>
public sealed class FileImageSource : IImageSource
{
    /// <inheritdoc />
    public Task<Stream> LoadAsync(ImageInput input, CancellationToken cancellationToken = default)
    {
        if (input.SourceKind != ImageSourceKind.File)
        {
            throw new ArgumentException("Expected a file input.", nameof(input));
        }

        if (string.IsNullOrWhiteSpace(input.FilePath))
        {
            throw new ArgumentException("File path is missing.", nameof(input));
        }

        if (!File.Exists(input.FilePath))
        {
            throw new FileNotFoundException($"Image file not found: {input.FilePath}", input.FilePath);
        }

        Stream stream = File.OpenRead(input.FilePath);
        return Task.FromResult(stream);
    }
}
