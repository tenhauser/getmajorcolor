using System.Linq;
using GetMajorColors.Models;
using GetMajorColors.Services;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace GetMajorColors.Tests.Unit;

public class ColorConversionTests
{
    [Theory]
    [InlineData(255, 0, 0, "r", 255)]
    [InlineData(0, 255, 0, "g", 255)]
    [InlineData(0, 0, 255, "b", 255)]
    public void ToRgb_ReturnsExpectedComponents(byte r, byte g, byte b, string component, double expected)
    {
        var pixel = new Rgb24(r, g, b);
        var values = ColorConverter.ToModel(pixel, ColorModel.Rgb);
        Assert.Equal(expected, values[component]);
    }

    [Fact]
    public void ToCmyk_Red_ReturnsCyanZero()
    {
        var pixel = new Rgb24(255, 0, 0);
        var values = ColorConverter.ToModel(pixel, ColorModel.Cmyk);

        Assert.Equal(0.0, values["c"]);
        Assert.Equal(100.0, values["m"]);
        Assert.Equal(100.0, values["y"]);
        Assert.Equal(0.0, values["k"]);
    }

    [Fact]
    public void ToHsl_Red_ReturnsHueZero()
    {
        var pixel = new Rgb24(255, 0, 0);
        var values = ColorConverter.ToModel(pixel, ColorModel.Hsl);

        Assert.Equal(0.0, values["h"]);
        Assert.True(values["s"] > 0.99);
        Assert.Equal(0.5, values["l"], precision: 2);
    }

    [Fact]
    public void ToHex_Red_ReturnsNumericHex()
    {
        var pixel = new Rgb24(255, 0, 0);
        var values = ColorConverter.ToModel(pixel, ColorModel.Hex);

        Assert.Equal(0xFF0000, values["hex"]);
    }

    [Fact]
    public void ToHexString_Red_ReturnsPaddedString()
    {
        var pixel = new Rgb24(255, 0, 0);
        Assert.Equal("#FF0000", ColorConverter.ToHexString(pixel));
    }
}
