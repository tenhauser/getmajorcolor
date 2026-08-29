using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Clipboard;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace GetMajorColors.Tests.Unit;

public class ClipboardProviderTests
{
    [Fact]
    public void CreateClipboardProvider_OnWindows_ReturnsWindowsProvider()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        IClipboardImageProvider provider = ClipboardTestHelpers.CreateProviderForCurrentOS();
        Assert.IsType<WindowsClipboardImageProvider>(provider);
    }

    [Fact]
    public void CreateClipboardProvider_OnLinux_ReturnsLinuxProvider()
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        IClipboardImageProvider provider = ClipboardTestHelpers.CreateProviderForCurrentOS();
        Assert.IsType<LinuxClipboardImageProvider>(provider);
    }

    [Fact]
    public void CreateClipboardProvider_OnMacOS_ReturnsMacOSProvider()
    {
        if (!OperatingSystem.IsMacOS())
        {
            return;
        }

        IClipboardImageProvider provider = ClipboardTestHelpers.CreateProviderForCurrentOS();
        Assert.IsType<MacOSClipboardImageProvider>(provider);
    }
}
