using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Clipboard;
using GetMajorColors.Models;
using GetMajorColors.Output;
using GetMajorColors.Services;
using SixLabors.ImageSharp.PixelFormats;

namespace GetMajorColors.Commands;

/// <summary>
/// Parses command-line arguments and executes image color analysis.
/// </summary>
public static class AnalyzeCommand
{
    /// <summary>
    /// Parses arguments and runs the analysis.
    /// </summary>
    public static async Task<int> RunAsync(string[] args, TextWriter stdout, TextWriter stderr, CancellationToken cancellationToken = default)
    {
        var parseResult = ParseArgs(args);

        if (parseResult.ShowHelp)
        {
            await stdout.WriteLineAsync(GetHelpText()).ConfigureAwait(false);
            return 0;
        }

        if (parseResult.ShowVersion)
        {
            await stdout.WriteLineAsync("getmajorcolors 1.0.0").ConfigureAwait(false);
            return 0;
        }

        if (parseResult.Error is not null)
        {
            await stderr.WriteLineAsync($"Error: {parseResult.Error}").ConfigureAwait(false);
            if (parseResult.Error.Contains("color model", StringComparison.OrdinalIgnoreCase))
            {
                return 5;
            }

            if (parseResult.Error.Contains("output format", StringComparison.OrdinalIgnoreCase))
            {
                return 6;
            }

            return 1;
        }

        return await ExecuteAsync(
            parseResult.FilePath,
            parseResult.Clipboard,
            parseResult.Count,
            parseResult.Model,
            parseResult.Visual ? OutputFormat.Visual : parseResult.Format,
            stdout,
            stderr,
            cancellationToken).ConfigureAwait(false);
    }

    public static async Task<int> ExecuteAsync(
        string? filePath,
        bool clipboard,
        int count,
        ColorModel model,
        OutputFormat format,
        TextWriter stdout,
        TextWriter stderr,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (count < 1)
            {
                await stderr.WriteLineAsync("Error: --count must be at least 1.").ConfigureAwait(false);
                return 13;
            }

            var input = ResolveInput(filePath, clipboard);

            IImageSource source = input.SourceKind switch
            {
                ImageSourceKind.File => new FileImageSource(),
                ImageSourceKind.StandardInput => new StdinImageSource(),
                ImageSourceKind.Clipboard => new ClipboardImageSource(CreateClipboardProvider()),
                _ => throw new InvalidOperationException("Unsupported input source."),
            };

            using Stream imageStream = await source.LoadAsync(input, cancellationToken).ConfigureAwait(false);

            var analyzer = new ImageSharpColorAnalyzer(new CssColorNameResolver());
            IReadOnlyList<MajorColor> colors = await analyzer.AnalyzeAsync(imageStream, count, cancellationToken).ConfigureAwait(false);

            var convertedColors = colors.Select(c => ConvertModel(c, model)).ToList();

            IOutputWriter writer = format switch
            {
                OutputFormat.Text => new TextOutputWriter(),
                OutputFormat.Json => new JsonOutputWriter(),
                OutputFormat.Jsonl => new JsonlOutputWriter(),
                OutputFormat.Yaml => new YamlOutputWriter(),
                OutputFormat.Visual => new VisualOutputWriter(),
                _ => throw new NotSupportedException($"Output format '{format}' is not supported."),
            };

            await writer.WriteAsync(stdout, convertedColors, model, cancellationToken).ConfigureAwait(false);
            return 0;
        }
        catch (FileNotFoundException ex)
        {
            await stderr.WriteLineAsync($"Error: {ex.Message}").ConfigureAwait(false);
            return 2;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("clipboard", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("Clipboard", StringComparison.OrdinalIgnoreCase))
        {
            await stderr.WriteLineAsync($"Error: {ex.Message}").ConfigureAwait(false);
            return 3;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Standard input is empty", StringComparison.Ordinal))
        {
            await stderr.WriteLineAsync($"Error: {ex.Message}").ConfigureAwait(false);
            return 4;
        }
        catch (NotSupportedException ex) when (ex.Message.Contains("Color model", StringComparison.OrdinalIgnoreCase))
        {
            await stderr.WriteLineAsync($"Error: {ex.Message}").ConfigureAwait(false);
            return 5;
        }
        catch (NotSupportedException ex) when (ex.Message.Contains("Output format", StringComparison.OrdinalIgnoreCase))
        {
            await stderr.WriteLineAsync($"Error: {ex.Message}").ConfigureAwait(false);
            return 6;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Terminal does not support", StringComparison.Ordinal))
        {
            await stderr.WriteLineAsync($"Error: {ex.Message}").ConfigureAwait(false);
            return 7;
        }
        catch (Exception ex) when (ex is InvalidOperationException or SixLabors.ImageSharp.InvalidImageContentException or SixLabors.ImageSharp.UnknownImageFormatException)
        {
            await stderr.WriteLineAsync($"Error: {ex.Message}").ConfigureAwait(false);
            return 8;
        }
        catch (Exception ex)
        {
            await stderr.WriteLineAsync($"Error: {ex.Message}").ConfigureAwait(false);
            return 1;
        }
    }

    private static ImageInput ResolveInput(string? filePath, bool clipboard)
    {
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            return new ImageInput(ImageSourceKind.File, filePath);
        }

        if (clipboard)
        {
            return new ImageInput(ImageSourceKind.Clipboard);
        }

        return new ImageInput(ImageSourceKind.StandardInput, Stream: Console.OpenStandardInput());
    }

    private static IClipboardImageProvider CreateClipboardProvider()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsClipboardImageProvider();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new LinuxClipboardImageProvider();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new MacOSClipboardImageProvider();
        }

        throw new InvalidOperationException("Clipboard access is not supported on this operating system.");
    }

    private static MajorColor ConvertModel(MajorColor color, ColorModel model)
    {
        if (color.Values.Model == model)
        {
            return color;
        }

        var rgb = ValuesToRgb(color.Values);
        var convertedValues = new ColorValues(model, ColorConverter.ToModel(rgb, model));
        return color with { Values = convertedValues };
    }

    private static Rgb24 ValuesToRgb(ColorValues values)
    {
        byte r = (byte)values.Components.GetValueOrDefault("r", 0);
        byte g = (byte)values.Components.GetValueOrDefault("g", 0);
        byte b = (byte)values.Components.GetValueOrDefault("b", 0);
        return new Rgb24(r, g, b);
    }

    private static ParseResult ParseArgs(string[] args)
    {
        var result = new ParseResult();
        int i = 0;

        while (i < args.Length)
        {
            string arg = args[i];

            if (arg is "--help" or "-h")
            {
                result.ShowHelp = true;
                return result;
            }

            if (arg is "--version")
            {
                result.ShowVersion = true;
                return result;
            }

            if (arg is "--clipboard" or "-c")
            {
                result.Clipboard = true;
                i++;
                continue;
            }

            if (arg is "--visual" or "-v")
            {
                result.Visual = true;
                i++;
                continue;
            }

            if ((arg is "--count" or "-n") && i + 1 < args.Length)
            {
                if (!int.TryParse(args[i + 1], out int count))
                {
                    result.Error = $"Invalid value for {arg}: '{args[i + 1]}'. Expected a positive integer.";
                    return result;
                }

                result.Count = count;
                i += 2;
                continue;
            }

            if ((arg is "--model" or "-m") && i + 1 < args.Length)
            {
                if (!TryParseModel(args[i + 1], out ColorModel model))
                {
                    result.Error = $"Unsupported color model '{args[i + 1]}'. Supported models: rgb, cmyk, hsl, hex.";
                    return result;
                }

                result.Model = model;
                i += 2;
                continue;
            }

            if ((arg is "--format" or "-f") && i + 1 < args.Length)
            {
                if (!TryParseFormat(args[i + 1], out OutputFormat format))
                {
                    result.Error = $"Unsupported output format '{args[i + 1]}'. Supported formats: text, json, jsonl, yaml, visual.";
                    return result;
                }

                result.Format = format;
                i += 2;
                continue;
            }

            if (!arg.StartsWith('-'))
            {
                if (result.FilePath is not null)
                {
                    result.Error = "Multiple file paths are not supported.";
                    return result;
                }

                result.FilePath = arg;
                i++;
                continue;
            }

            result.Error = $"Unknown option: {arg}";
            return result;
        }

        return result;
    }

    private static bool TryParseModel(string value, out ColorModel model)
    {
        switch (value.ToLowerInvariant())
        {
            case "rgb":
                model = ColorModel.Rgb;
                return true;
            case "cmyk":
                model = ColorModel.Cmyk;
                return true;
            case "hsl":
                model = ColorModel.Hsl;
                return true;
            case "hex":
                model = ColorModel.Hex;
                return true;
            default:
                model = default;
                return false;
        }
    }

    private static bool TryParseFormat(string value, out OutputFormat format)
    {
        switch (value.ToLowerInvariant())
        {
            case "text":
                format = OutputFormat.Text;
                return true;
            case "json":
                format = OutputFormat.Json;
                return true;
            case "jsonl":
                format = OutputFormat.Jsonl;
                return true;
            case "yaml":
                format = OutputFormat.Yaml;
                return true;
            case "visual":
                format = OutputFormat.Visual;
                return true;
            default:
                format = default;
                return false;
        }
    }

    private static string GetHelpText()
    {
        return """
            getmajorcolors [options] [<image-file>]

            Analyze the major colors in an image.

            Options:
              -c, --clipboard          Read the image from the system clipboard.
              -n, --count <number>     Number of major colors to report (default: 1).
              -m, --model <model>      Color model: rgb, cmyk, hsl, hex (default: rgb).
              -f, --format <format>    Output format: text, json, jsonl, yaml, visual (default: text).
              -v, --visual             Render ANSI truecolor swatches.
              -h, --help               Show this help message.
              --version                Show version information.
            """;
    }

    private sealed class ParseResult
    {
        public string? FilePath { get; set; }
        public bool Clipboard { get; set; }
        public int Count { get; set; } = 1;
        public ColorModel Model { get; set; } = ColorModel.Rgb;
        public OutputFormat Format { get; set; } = OutputFormat.Text;
        public bool Visual { get; set; }
        public bool ShowHelp { get; set; }
        public bool ShowVersion { get; set; }
        public string? Error { get; set; }
    }
}
