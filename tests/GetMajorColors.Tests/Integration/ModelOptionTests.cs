using System.IO;
using System.Threading.Tasks;
using GetMajorColors.Commands;
using GetMajorColors.Models;
using Xunit;

namespace GetMajorColors.Tests.Integration;

public class ModelOptionTests
{
    public ModelOptionTests()
    {
        SampleImages.EnsureExists();
    }

    [Theory]
    [InlineData(ColorModel.Rgb, "rgb(255, 0, 0)")]
    [InlineData(ColorModel.Hex, "#FF0000")]
    public async Task ValidModel_ReturnsZero(ColorModel model, string expectedFragment)
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            SampleImages.Paths.SolidRed,
            clipboard: false,
            count: 1,
            model: model,
            format: OutputFormat.Text,
            stdout,
            stderr);

        Assert.Equal(0, exitCode);
        Assert.Contains(expectedFragment, stdout.ToString());
    }

    [Fact]
    public async Task CmykModel_ReturnsCyanZeroForRed()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            SampleImages.Paths.SolidRed,
            clipboard: false,
            count: 1,
            model: ColorModel.Cmyk,
            format: OutputFormat.Text,
            stdout,
            stderr);

        string output = stdout.ToString();
        Assert.Equal(0, exitCode);
        Assert.Contains("cmyk(0, 100, 100, 0)", output);
    }
}
