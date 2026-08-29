using SixLabors.ImageSharp.PixelFormats;

namespace GetMajorColors.Services;

/// <summary>
/// Resolves a color to its closest human-readable name.
/// </summary>
public interface IColorNameResolver
{
    /// <summary>
    /// Returns the closest named color for the given RGB pixel.
    /// </summary>
    /// <param name="pixel">The RGB color.</param>
    /// <returns>The closest named color.</returns>
    string GetName(Rgb24 pixel);
}
