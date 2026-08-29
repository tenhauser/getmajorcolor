using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GetMajorColors.Clipboard;

/// <summary>
/// Reads image data from the Windows clipboard by saving it to a temporary PNG file via PowerShell.
/// </summary>
public sealed class WindowsClipboardImageProvider : IClipboardImageProvider
{
    /// <inheritdoc />
    public async Task<Stream?> GetImageAsync(CancellationToken cancellationToken = default)
    {
        string tempPath = Path.Combine(Path.GetTempPath(), $"getmajorcolors-{Guid.NewGuid()}.png");

        try
        {
            string script = $@"
Add-Type -AssemblyName System.Windows.Forms
$img = [Windows.Forms.Clipboard]::GetImage()
if ($img -ne $null) {{
    $img.Save('{tempPath.Replace("'", "''")}')
}}
";

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script.Replace("\"", "\\\"")}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            process.Start();
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

            if (!File.Exists(tempPath))
            {
                return null;
            }

            var bytes = await File.ReadAllBytesAsync(tempPath, cancellationToken).ConfigureAwait(false);
            return new MemoryStream(bytes, writable: false);
        }
        finally
        {
            TryDelete(tempPath);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }
    }
}
