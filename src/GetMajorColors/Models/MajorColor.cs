using System.Collections.Generic;

namespace GetMajorColors.Models;

/// <summary>
/// Numeric color values expressed in a specific color model.
/// </summary>
/// <param name="Model">The color model used for the values.</param>
/// <param name="Components">Model-specific component values.</param>
public sealed record ColorValues(ColorModel Model, IReadOnlyDictionary<string, double> Components);

/// <summary>
/// A dominant color identified in an image, including its name, numeric values, and coverage.
/// </summary>
/// <param name="Name">The closest human-readable color name.</param>
/// <param name="Values">Numeric values in the selected color model.</param>
/// <param name="Coverage">Relative presence in the image, from 0.0 to 1.0.</param>
public sealed record MajorColor(string Name, ColorValues Values, double Coverage);
