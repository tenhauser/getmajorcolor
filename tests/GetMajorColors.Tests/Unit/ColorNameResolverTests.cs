using GetMajorColors.Services;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace GetMajorColors.Tests.Unit;

public class ColorNameResolverTests
{
    private readonly CssColorNameResolver _resolver = new();

    [Theory]
    [InlineData(255, 0, 0, "Red")]
    [InlineData(0, 128, 0, "Green")]
    [InlineData(0, 0, 255, "Blue")]
    [InlineData(255, 255, 255, "White")]
    [InlineData(0, 0, 0, "Black")]
    public void GetName_ReturnsExpectedName(byte r, byte g, byte b, string expected)
    {
        string actual = _resolver.GetName(new Rgb24(r, g, b));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetName_NearRed_ReturnsRedOrCloseName()
    {
        string name = _resolver.GetName(new Rgb24(250, 5, 5));
        Assert.Equal("Red", name);
    }
}
