using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GetMajorColors.Clipboard;

namespace GetMajorColors.Tests;

/// <summary>
/// Helpers for clipboard-related tests.
/// </summary>
public static class ClipboardTestHelpers
{
    /// <summary>
    /// Creates the platform-specific clipboard provider used by production code.
    /// Mirrors the selection logic in <see cref="GetMajorColors.Commands.AnalyzeCommand"/>.
    /// </summary>
    public static IClipboardImageProvider CreateProviderForCurrentOS()
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsClipboardImageProvider();
        }

        if (OperatingSystem.IsLinux())
        {
            return new LinuxClipboardImageProvider();
        }

        if (OperatingSystem.IsMacOS())
        {
            return new MacOSClipboardImageProvider();
        }

        throw new PlatformNotSupportedException("Clipboard access is not supported on this operating system.");
    }

    /// <summary>
    /// Creates a stub clipboard provider that returns the supplied image bytes.
    /// </summary>
    public static IClipboardImageProvider CreateStub(byte[] imageBytes)
    {
        return new StubClipboardImageProvider(imageBytes);
    }

    private sealed class StubClipboardImageProvider : IClipboardImageProvider
    {
        private readonly byte[] _imageBytes;

        public StubClipboardImageProvider(byte[] imageBytes)
        {
            _imageBytes = imageBytes;
        }

        public Task<Stream?> GetImageAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Stream?>(new MemoryStream(_imageBytes, writable: false));
        }
    }
}
