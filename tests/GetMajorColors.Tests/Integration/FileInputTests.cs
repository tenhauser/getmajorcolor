using System;
using System.IO;
using System.Threading.Tasks;
using GetMajorColors.Commands;
using GetMajorColors.Models;
using GetMajorColors.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace GetMajorColors.Tests.Integration;

public class FileInputTests : IDisposable
{
    private readonly string _tempDirectory;

    public FileInputTests()
    {
        SampleImages.EnsureExists();
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"getmajorcolors-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }
        catch
        {
            // best-effort cleanup
        }
    }

    [Fact]
    public async Task SolidRedImage_ReturnsRedWithFullCoverage()
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

        string output = stdout.ToString();
        Assert.Equal(0, exitCode);
        Assert.Contains("Red", output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("rgb(255, 0, 0)", output);
        Assert.Contains("100.0%", output);
    }

    [Fact]
    public async Task MissingFile_ReturnsExitCodeTwo()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        string missingPath = Path.Combine(_tempDirectory, "does-not-exist.png");

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            missingPath,
            clipboard: false,
            count: 1,
            model: ColorModel.Rgb,
            format: OutputFormat.Text,
            stdout,
            stderr);

        Assert.Equal(2, exitCode);
        Assert.Contains("not found", stderr.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UnsupportedFile_ReturnsExitCodeEight()
    {
        string badFile = Path.Combine(_tempDirectory, "not-an-image.txt");
        await File.WriteAllTextAsync(badFile, "this is not an image");

        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            badFile,
            clipboard: false,
            count: 1,
            model: ColorModel.Rgb,
            format: OutputFormat.Text,
            stdout,
            stderr);

        Assert.Equal(8, exitCode);
    }

    [Fact]
    public async Task GradientImage_ReturnsMultipleColors_WhenCountGreaterThanOne()
    {
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        int exitCode = await AnalyzeCommand.ExecuteAsync(
            SampleImages.Paths.GradientRgb,
            clipboard: false,
            count: 2,
            model: ColorModel.Rgb,
            format: OutputFormat.Text,
            stdout,
            stderr);

        string output = stdout.ToString();
        Assert.Equal(0, exitCode);
        Assert.True(output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length >= 2, output);
    }
}
