using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace GetMajorColors.Services;

/// <summary>
/// Identifies major colors using ImageSharp quantization and pixel histograms.
/// </summary>
public sealed class ImageSharpColorAnalyzer : IColorAnalyzer
{
    private const int MaxDimension = 512;
    private const long MaxInputBytes = 100 * 1024 * 1024;

    private readonly IColorNameResolver _nameResolver;

    /// <summary>
    /// Creates a new analyzer using the supplied name resolver.
    /// </summary>
    public ImageSharpColorAnalyzer(IColorNameResolver nameResolver)
    {
        _nameResolver = nameResolver;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MajorColor>> AnalyzeAsync(Stream imageStream, int count, CancellationToken cancellationToken = default)
    {
        if (imageStream.Length > MaxInputBytes)
        {
            throw new InvalidOperationException($"Input image exceeds maximum size of {MaxInputBytes / (1024 * 1024)} MB.");
        }

        using var image = await Image.LoadAsync<Rgb24>(imageStream, cancellationToken).ConfigureAwait(false);

        if (image.Width > MaxDimension || image.Height > MaxDimension)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(MaxDimension, MaxDimension),
            }));
        }

        int paletteSize = Math.Max(count + 1, 2);
        var quantizer = new OctreeQuantizer(new QuantizerOptions { MaxColors = paletteSize });

        using var quantized = image.Clone(x => x.Quantize(quantizer));

        var histogram = new Dictionary<Rgb24, int>();
        int totalPixels = quantized.Width * quantized.Height;

        quantized.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgb24> row = accessor.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                {
                    Rgb24 pixel = row[x];
                    histogram.TryGetValue(pixel, out int current);
                    histogram[pixel] = current + 1;
                }
            }
        });

        var sorted = histogram
            .OrderByDescending(kvp => kvp.Value)
            .Take(count)
            .ToList();

        var results = new List<MajorColor>(sorted.Count);
        foreach (var (pixel, pixelCount) in sorted)
        {
            double coverage = (double)pixelCount / totalPixels;
            var values = new ColorValues(ColorModel.Rgb, ColorConverter.ToModel(pixel, ColorModel.Rgb));
            results.Add(new MajorColor(_nameResolver.GetName(pixel), values, coverage));
        }

        return results;
    }
}
