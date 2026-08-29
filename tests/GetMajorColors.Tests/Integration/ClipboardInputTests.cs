using System.IO;
using System.Threading.Tasks;
using GetMajorColors.Clipboard;
using GetMajorColors.Models;
using GetMajorColors.Services;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace GetMajorColors.Tests.Integration;

public class ClipboardInputTests
{
    [Fact]
    public async Task ClipboardWithImage_ReturnsMajorColor()
    {
        byte[] imageBytes = TestImageGenerator.CreateSolidColor(100, 100, new Rgb24(0, 0, 255));
        IClipboardImageProvider stub = ClipboardTestHelpers.CreateStub(imageBytes);
        var source = new ClipboardImageSource(stub);

        using Stream stream = await source.LoadAsync(new ImageInput(ImageSourceKind.Clipboard));
        var analyzer = new ImageSharpColorAnalyzer(new CssColorNameResolver());
        var colors = await analyzer.AnalyzeAsync(stream, count: 1);

        Assert.Single(colors);
        Assert.Contains("Blue", colors[0].Name, System.StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1.0, colors[0].Coverage);
    }
}
