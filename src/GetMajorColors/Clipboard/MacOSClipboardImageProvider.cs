using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GetMajorColors.Clipboard;

/// <summary>
/// Reads image data from the macOS clipboard using pngpaste.
/// </summary>
public sealed class MacOSClipboardImageProvider : IClipboardImageProvider
{
    /// <inheritdoc />
    public async Task<Stream?> GetImageAsync(CancellationToken cancellationToken = default)
    {
        if (!IsCommandAvailable("pngpaste"))
        {
            throw new InvalidOperationException("Clipboard access on macOS requires 'pngpaste'. Install it (e.g., 'brew install pngpaste') or use a file path or standard input instead.");
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "pngpaste",
                Arguments = "-",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        var memoryStream = new MemoryStream();
        process.Start();
        await process.StandardOutput.BaseStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

        if (process.ExitCode != 0 || memoryStream.Length == 0)
        {
            return null;
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    private static bool IsCommandAvailable(string command)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "command",
                    Arguments = $"-v {command}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
