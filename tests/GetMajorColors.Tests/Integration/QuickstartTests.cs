using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GetMajorColors.Commands;
using GetMajorColors.Models;
using GetMajorColors.Services;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;
using YamlDotNet.Serialization;

namespace GetMajorColors.Tests.Integration;

public class QuickstartTests
{
    public QuickstartTests()
    {
        SampleImages.EnsureExists();
    }

    [Fact]
    public async Task FileInput_RedImage_NamesRedWithFullCoverage()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            SampleImages.Paths.SolidRed,
            clipboard: false,
            count: 1,
            model: ColorModel.Rgb,
            format: OutputFormat.Text,
            stdout,
            stderr);

        Assert.Equal(0, exitCode);
        Assert.Contains("Red", stdout.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Gradient_CountThree_CoveragesSumToOne()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            SampleImages.Paths.GradientRgb,
            clipboard: false,
            count: 3,
            model: ColorModel.Rgb,
            format: OutputFormat.Text,
            stdout,
            stderr);

        Assert.Equal(0, exitCode);
        string output = stdout.ToString();
        var percentages = output.Split('%', StringSplitOptions.RemoveEmptyEntries)
            .Select(s =>
            {
                int space = s.LastIndexOf(' ');
                return double.TryParse(space >= 0 ? s[(space + 1)..] : s, out double v) ? v : 0.0;
            })
            .Where(v => v > 0)
            .ToList();

        Assert.True(percentages.Count <= 3);
    }

    [Fact]
    public async Task Stdin_RedImage_ReturnsValidJson()
    {
        byte[] bytes = await File.ReadAllBytesAsync(SampleImages.Paths.SolidRed);
        var source = new StdinImageSource();

        using Stream stream = await source.LoadAsync(new ImageInput(ImageSourceKind.StandardInput, Stream: new MemoryStream(bytes)));
        var analyzer = new ImageSharpColorAnalyzer(new CssColorNameResolver());
        var colors = await analyzer.AnalyzeAsync(stream, 1);

        Assert.Single(colors);
        Assert.Contains("Red", colors[0].Name, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task JsonlOutput_IsParseablePerLine()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            SampleImages.Paths.GradientRgb,
            clipboard: false,
            count: 2,
            model: ColorModel.Rgb,
            format: OutputFormat.Jsonl,
            stdout,
            stderr);

        Assert.Equal(0, exitCode);
        string[] lines = stdout.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.True(lines.Length <= 2);
        Assert.All(lines, line => JsonDocument.Parse(line));
    }

    [Fact]
    public async Task YamlOutput_IsValidYaml()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            SampleImages.Paths.SolidRed,
            clipboard: false,
            count: 1,
            model: ColorModel.Rgb,
            format: OutputFormat.Yaml,
            stdout,
            stderr);

        Assert.Equal(0, exitCode);
        var deserializer = new DeserializerBuilder().Build();
        var document = deserializer.Deserialize<Dictionary<string, object>>(stdout.ToString());
        Assert.NotNull(document);
        Assert.True(document.ContainsKey("colors"));
    }

    [Fact]
    public async Task CmykModel_Json_HasCmykKeys()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            SampleImages.Paths.SolidRed,
            clipboard: false,
            count: 1,
            model: ColorModel.Cmyk,
            format: OutputFormat.Json,
            stdout,
            stderr);

        Assert.Equal(0, exitCode);
        using var document = JsonDocument.Parse(stdout.ToString());
        JsonElement values = document.RootElement
            .GetProperty("colors")[0]
            .GetProperty("values");

        Assert.True(values.TryGetProperty("c", out _));
        Assert.True(values.TryGetProperty("m", out _));
        Assert.True(values.TryGetProperty("y", out _));
        Assert.True(values.TryGetProperty("k", out _));
    }

    [Fact]
    public async Task MissingFile_ReturnsExitCodeTwo()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            "/does/not/exist.png",
            clipboard: false,
            count: 1,
            model: ColorModel.Rgb,
            format: OutputFormat.Text,
            stdout,
            stderr);

        Assert.Equal(2, exitCode);
    }

    [Fact]
    public async Task UnsupportedModel_ReturnsExitCodeFive()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.RunAsync(
            new[] { "--model", "xyz", SampleImages.Paths.SolidRed },
            stdout,
            stderr);

        Assert.Equal(5, exitCode);
    }

    [Fact]
    public async Task EmptyStdin_ReturnsExitCodeFour()
    {
        var source = new StdinImageSource();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => source.LoadAsync(new ImageInput(ImageSourceKind.StandardInput, Stream: new MemoryStream())));

        Assert.Contains("empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
