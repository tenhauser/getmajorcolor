using System.IO;

namespace GetMajorColors.Models;

/// <summary>
/// Identifies the source from which an image is loaded.
/// </summary>
public enum ImageSourceKind
{
    File,
    Clipboard,
    StandardInput,
}

/// <summary>
/// Represents the source image provided by the user.
/// </summary>
/// <param name="SourceKind">The kind of image source.</param>
/// <param name="FilePath">Path to the image file when <see cref="SourceKind"/> is <see cref="ImageSourceKind.File"/>.</param>
/// <param name="Stream">Readable image stream when source is not a file.</param>
public sealed record ImageInput(ImageSourceKind SourceKind, string? FilePath = null, Stream? Stream = null);
