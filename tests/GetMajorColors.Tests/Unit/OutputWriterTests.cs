using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GetMajorColors.Models;
using GetMajorColors.Output;
using GetMajorColors.Services;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;
using YamlDotNet.Serialization;

namespace GetMajorColors.Tests.Unit;

public class OutputWriterTests
{
    private static MajorColor CreateColor(string name, byte r, byte g, byte b, double coverage)
    {
        var pixel = new Rgb24(r, g, b);
        var values = new ColorValues(ColorModel.Rgb, ColorConverter.ToModel(pixel, ColorModel.Rgb));
        return new MajorColor(name, values, coverage);
    }

    [Fact]
    public async Task TextOutputWriter_WritesNameAndCoverage()
    {
        var color = CreateColor("Red", 255, 0, 0, 1.0);
        var writer = new TextOutputWriter();
        using var sw = new StringWriter();

        await writer.WriteAsync(sw, new[] { color }, ColorModel.Rgb);

        string output = sw.ToString();
        Assert.Contains("Red", output);
        Assert.Contains("rgb(255, 0, 0)", output);
        Assert.Contains("100.0%", output);
    }

    [Fact]
    public async Task JsonOutputWriter_WritesValidJson()
    {
        var color = CreateColor("Blue", 0, 0, 255, 0.5);
        var writer = new JsonOutputWriter();
        using var sw = new StringWriter();

        await writer.WriteAsync(sw, new[] { color }, ColorModel.Rgb);

        string output = sw.ToString();
        using var document = JsonDocument.Parse(output);
        var colors = document.RootElement.GetProperty("colors").EnumerateArray().ToList();
        Assert.Single(colors);
        Assert.Equal("Blue", colors[0].GetProperty("name").GetString());
    }

    [Fact]
    public async Task JsonlOutputWriter_WritesOneLinePerColor()
    {
        var colors = new List<MajorColor>
        {
            CreateColor("Red", 255, 0, 0, 0.6),
            CreateColor("Blue", 0, 0, 255, 0.4),
        };
        var writer = new JsonlOutputWriter();
        using var sw = new StringWriter();

        await writer.WriteAsync(sw, colors, ColorModel.Rgb);

        string[] lines = sw.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.All(lines, line => JsonDocument.Parse(line));
    }

    [Fact]
    public async Task YamlOutputWriter_WritesValidYaml()
    {
        var color = CreateColor("Green", 0, 128, 0, 1.0);
        var writer = new YamlOutputWriter();
        using var sw = new StringWriter();

        await writer.WriteAsync(sw, new[] { color }, ColorModel.Rgb);

        string output = sw.ToString();
        var deserializer = new DeserializerBuilder().Build();
        var document = deserializer.Deserialize<Dictionary<string, List<Dictionary<string, object>>>>(output);
        Assert.NotNull(document);
        Assert.True(document.ContainsKey("colors"));
    }

    [Fact]
    public async Task VisualOutputWriter_SupportedTerminal_WritesSwatch()
    {
        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        var originalTerm = Environment.GetEnvironmentVariable("TERM");
        Environment.SetEnvironmentVariable("NO_COLOR", null);
        Environment.SetEnvironmentVariable("TERM", "xterm-256color");

        try
        {
            var color = CreateColor("Red", 255, 0, 0, 1.0);
            var writer = new VisualOutputWriter();
            using var sw = new StringWriter();

            await writer.WriteAsync(sw, new[] { color }, ColorModel.Rgb);

            Assert.Contains("\x1b[48;2;255;0;0m", sw.ToString());
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
            Environment.SetEnvironmentVariable("TERM", originalTerm);
        }
    }

    [Fact]
    public async Task VisualOutputWriter_NoColorSet_ThrowsUnsupported()
    {
        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        Environment.SetEnvironmentVariable("NO_COLOR", "1");

        try
        {
            var color = CreateColor("Red", 255, 0, 0, 1.0);
            var writer = new VisualOutputWriter();
            using var sw = new StringWriter();

            await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteAsync(sw, new[] { color }, ColorModel.Rgb));
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }
}
