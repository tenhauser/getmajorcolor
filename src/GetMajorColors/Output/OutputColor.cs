using System.Collections.Generic;
using GetMajorColors.Models;

namespace GetMajorColors.Output;

/// <summary>
/// Serializable representation of a major color for output writers.
/// </summary>
public sealed record OutputColor(
    string Name,
    double Coverage,
    string Model,
    IReadOnlyDictionary<string, double> Values);
