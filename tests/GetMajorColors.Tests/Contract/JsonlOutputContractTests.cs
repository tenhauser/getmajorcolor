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

namespace GetMajorColors.Tests.Contract;

public class JsonlOutputContractTests
{
    private static MajorColor CreateColor(string name, byte r, byte g, byte b, double coverage)
    {
        var pixel = new Rgb24(r, g, b);
        var values = new ColorValues(ColorModel.Rgb, ColorConverter.ToModel(pixel, ColorModel.Rgb));
        return new MajorColor(name, values, coverage);
    }

    [Fact]
    public async Task EachLineIsValidJsonWithRequiredFields()
    {
        var colors = new List<MajorColor>
        {
            CreateColor("Crimson", 220, 20, 60, 0.452),
            CreateColor("White", 255, 255, 255, 0.328),
        };

        var writer = new JsonlOutputWriter();
        using var sw = new StringWriter();
        await writer.WriteAsync(sw, colors, ColorModel.Rgb);

        string[] lines = sw.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);

        foreach (string line in lines)
        {
            using var document = JsonDocument.Parse(line);
            JsonElement root = document.RootElement;
            Assert.NotNull(root.GetProperty("name").GetString());
            Assert.True(root.GetProperty("coverage").GetDouble() > 0);
            Assert.Equal("rgb", root.GetProperty("model").GetString());
            Assert.Equal(3, root.GetProperty("values").EnumerateObject().Count());
        }
    }

    [Fact]
    public async Task CoveragesSumToWithinOnePercent()
    {
        var colors = new List<MajorColor>
        {
            CreateColor("Crimson", 220, 20, 60, 0.452),
            CreateColor("White", 255, 255, 255, 0.328),
            CreateColor("Black", 0, 0, 0, 0.220),
        };

        var writer = new JsonlOutputWriter();
        using var sw = new StringWriter();
        await writer.WriteAsync(sw, colors, ColorModel.Rgb);

        string[] lines = sw.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        double sum = lines.Select(line => JsonDocument.Parse(line).RootElement.GetProperty("coverage").GetDouble()).Sum();

        Assert.True(Math.Abs(sum - 1.0) <= 0.01, $"Coverage sum {sum} is outside 1% tolerance.");
    }
}
