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

public class JsonOutputContractTests
{
    private static MajorColor CreateColor(string name, byte r, byte g, byte b, double coverage)
    {
        var pixel = new Rgb24(r, g, b);
        var values = new ColorValues(ColorModel.Rgb, ColorConverter.ToModel(pixel, ColorModel.Rgb));
        return new MajorColor(name, values, coverage);
    }

    [Fact]
    public async Task WritesColorsArrayWithModelAndValues()
    {
        var color = CreateColor("Crimson", 220, 20, 60, 0.452);
        var writer = new JsonOutputWriter();
        using var sw = new StringWriter();

        await writer.WriteAsync(sw, new[] { color }, ColorModel.Rgb);

        using var document = JsonDocument.Parse(sw.ToString());
        var root = document.RootElement;
        var colors = root.GetProperty("colors").EnumerateArray().ToList();
        Assert.Single(colors);

        JsonElement first = colors[0];
        Assert.Equal("Crimson", first.GetProperty("name").GetString());
        Assert.Equal(0.452, first.GetProperty("coverage").GetDouble());
        Assert.Equal("rgb", first.GetProperty("model").GetString());

        JsonElement values = first.GetProperty("values");
        Assert.Equal(220, values.GetProperty("r").GetInt32());
        Assert.Equal(20, values.GetProperty("g").GetInt32());
        Assert.Equal(60, values.GetProperty("b").GetInt32());
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

        var writer = new JsonOutputWriter();
        using var sw = new StringWriter();
        await writer.WriteAsync(sw, colors, ColorModel.Rgb);

        using var document = JsonDocument.Parse(sw.ToString());
        double sum = document.RootElement.GetProperty("colors").EnumerateArray()
            .Sum(c => c.GetProperty("coverage").GetDouble());

        Assert.True(Math.Abs(sum - 1.0) <= 0.01, $"Coverage sum {sum} is outside 1% tolerance.");
    }
}
