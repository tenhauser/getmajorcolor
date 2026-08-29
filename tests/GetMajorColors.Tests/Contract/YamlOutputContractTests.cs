using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GetMajorColors.Models;
using GetMajorColors.Output;
using GetMajorColors.Services;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;
using YamlDotNet.Serialization;

namespace GetMajorColors.Tests.Contract;

public class YamlOutputContractTests
{
    private static MajorColor CreateColor(string name, byte r, byte g, byte b, double coverage)
    {
        var pixel = new Rgb24(r, g, b);
        var values = new ColorValues(ColorModel.Rgb, ColorConverter.ToModel(pixel, ColorModel.Rgb));
        return new MajorColor(name, values, coverage);
    }

    [Fact]
    public async Task WritesColorsListWithModelAndValues()
    {
        var color = CreateColor("Crimson", 220, 20, 60, 0.452);
        var writer = new YamlOutputWriter();
        using var sw = new StringWriter();

        await writer.WriteAsync(sw, new[] { color }, ColorModel.Rgb);

        var deserializer = new DeserializerBuilder().Build();
        var document = deserializer.Deserialize<Dictionary<string, List<Dictionary<string, object>>>>(sw.ToString());

        Assert.NotNull(document);
        List<Dictionary<string, object>> colors = document["colors"];
        Assert.Single(colors);

        Dictionary<string, object> first = colors[0];
        Assert.Equal("Crimson", first["name"]);
        Assert.Equal("rgb", first["model"]);
        Assert.True(first.ContainsKey("values"));
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

        var writer = new YamlOutputWriter();
        using var sw = new StringWriter();
        await writer.WriteAsync(sw, colors, ColorModel.Rgb);

        var deserializer = new DeserializerBuilder().Build();
        var document = deserializer.Deserialize<Dictionary<string, List<Dictionary<string, object>>>>(sw.ToString());
        double sum = document["colors"].Select(c => Convert.ToDouble(c["coverage"])).Sum();

        Assert.True(Math.Abs(sum - 1.0) <= 0.01, $"Coverage sum {sum} is outside 1% tolerance.");
    }
}
