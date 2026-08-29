# Research: Image Color Analysis CLI

## Decision: Implementation Language — C# 12 / .NET 8 LTS

**Rationale**: The user explicitly requested "dotnet core c#". .NET 8 is the current Long-Term Support (LTS) release, produces self-contained single-file executables, and is memory-safe by default. It satisfies the constitution's cross-platform, memory-safety, and CLI-first principles.

**Alternatives considered**:
- .NET 9 (STS): newer but shorter support lifecycle; LTS preferred for a release tool.
- .NET Framework: Windows-only, violates cross-platform requirement.

## Decision: Image Processing Library — SixLabors.ImageSharp

**Rationale**:
- Pure managed C# with no native dependencies, aligning with the constitution's cross-platform and memory-safety goals.
- Supports PNG, JPEG, GIF, BMP, WebP, TGA, and PBM.
- Provides built-in quantization (`OctreeQuantizer`, `WuQuantizer`) that can produce a fixed-size palette for major-color extraction.
- Active maintenance and permissive licensing for the required feature set.

**Alternatives considered**:
- `SkiaSharp`: powerful but requires native Skia binaries, complicating self-contained single-file releases.
- `System.Drawing.Common`: Windows-only on .NET 6+ and deprecated for cross-platform use.
- `Magick.NET`: ImageMagick wrapper with native dependencies; heavier than needed.

## Decision: Color Quantization Strategy — Octree Quantizer + Pixel Counting

**Rationale**:
- ImageSharp's `OctreeQuantizer` efficiently reduces an image to a small palette (e.g., `count + 1` colors to allow for a background/transparent bucket).
- After quantization, count how many pixels map to each palette entry to derive coverage percentages.
- For very large images, downscale to a bounded max dimension (e.g., 512px on the longest side) before quantization to meet the 5-second performance goal.

**Alternatives considered**:
- Custom k-means clustering: more flexible but more code, harder to guarantee deterministic results, and slower on large images.
- Wu quantizer: higher quality but slightly slower; Octree is sufficient for dominant-color analysis and can be swapped later behind the same interface.

## Decision: Clipboard Image Input — Platform-Specific Providers

**Rationale**:
- There is no lightweight, cross-platform, managed-only .NET clipboard library that supports bitmap/image data.
- The feature is required (FR-002), so the complexity is justified and isolated behind `IClipboardImageProvider`.
- Per-platform strategies:
  - **Windows**: Use `System.Runtime.InteropServices` to call `OpenClipboard`, `GetClipboardData(CF_DIBV5/CF_BITMAP)`, and `CloseClipboard`. Marshal bitmap bytes into a MemoryStream for ImageSharp.
  - **Linux**: Shell out to `xclip -selection clipboard -t image/png -o` and fall back to `xsel` if `xclip` is unavailable. Return the PNG bytes.
  - **macOS**: Shell out to `pngpaste -` (preferred) or `osascript` to read clipboard image data as PNG.
- Providers are selected at runtime using `RuntimeInformation.IsOSPlatform`.

**Alternatives considered**:
- Avalonia / GTK clipboard APIs: too heavy for a CLI tool.
- `TextCopy`: text-only, not applicable.
- Skip clipboard entirely: violates FR-002.

## Decision: Command-Line Parser — System.CommandLine

**Rationale**:
- Official .NET library with modern API, built-in help, completion, and validation.
- Supports commands, options, aliases, and arity without extra boilerplate.
- Aligns with the constitution's preference for widely maintained dependencies.

**Alternatives considered**:
- `CommandLineParser`: mature and stable, but `System.CommandLine` is the long-term direction from the .NET team.
- Manual `args` parsing: simpler but becomes unwieldy as options grow; violates simplicity in the long run.

## Decision: Output Formatters

**Rationale**:
- **JSON / JSONL**: Use `System.Text.Json` (built-in, fast, AOT-friendly).
- **YAML**: Use `YamlDotNet` (de facto standard for .NET YAML).
- **Text**: Custom formatter; simple string interpolation.
- **Visual**: Use ANSI 24-bit color escape sequences (`\e[48;2;r;g;bm`) where `NO_COLOR` is not set and the terminal reports truecolor support. Swatches are rendered as Unicode blocks with the color as background.

**Alternatives considered**:
- `Newtonsoft.Json` for JSON: excellent but adds a dependency where `System.Text.Json` is sufficient.
- Custom YAML writer: too error-prone; YamlDotNet is well-tested.

## Decision: Color Models — RGB (default), CMYK, HSL, HEX

**Rationale**:
- RGB is the native model from ImageSharp pixels and the most common default.
- CMYK is explicitly required (FR-009) for print workflows.
- HSL and HEX are popular with designers and developers and add minimal conversion code.
- Conversions are deterministic, well-known formulas implemented in a small static helper class.

**Alternatives considered**:
- LAB / LCH: more perceptually uniform but not requested; can be added later behind the same model enum.
- Pantone / RAL: proprietary or require large lookup tables; out of scope.

## Decision: Named-Color Resolution — CSS Color Module Level 4 Dictionary

**Rationale**:
- The CSS named color list is widely recognized, stable, and includes ~150 names.
- Nearest named color is found by minimum Euclidean distance in RGB space (fast) with an optional LAB distance for better perceptual matching if performance allows.
- If no close match exists within a threshold, the tool reports the closest name and the actual values, satisfying the "named color ambiguity" edge case.

**Alternatives considered**:
- X11 color names: larger list but includes obscure names and duplicates.
- Custom curated list: more maintenance; CSS list is a standard.

## Decision: Release Distribution — `dotnet publish` + GitHub Actions

**Rationale**:
- `dotnet publish -r <RID> -c Release --self-contained true -p:PublishSingleFile=true` produces the portable binaries required by the constitution and FR-016.
- GitHub Actions matrix builds for `win-x64`, `linux-x64`, and `osx-x64` can attach artifacts to a release.
- SHA-256 checksums are generated in the workflow (`sha256sum` / `Get-FileHash`) and uploaded alongside binaries.
- Install scripts (`install.sh`, `install.ps1`) download the correct binary, verify the checksum, and place it on `PATH`.
- The project is released under the MIT License (confirmed in spec), so release archives include a `LICENSE` file and any required third-party notices.

**Alternatives considered**:
- `dotnet tool` / NuGet distribution: requires .NET SDK installed, violating the self-contained binary goal.
- Snap / Homebrew / Chocolatey: valuable later but not required for first release and adds distribution complexity.

## Decision: Testing Strategy — xUnit + Known Sample Images

**Rationale**:
- xUnit is the most common .NET test framework and integrates with `dotnet test`.
- Unit tests cover color conversion, name resolution, and output writers without image I/O.
- Integration tests run the CLI against committed sample images (solid colors, gradients, photographs) and assert exit codes and output structure.
- Clipboard integration is tested manually or via platform-specific stubs because automating the real clipboard in CI is fragile.

## Resolved Unknowns

| Unknown | Resolution |
|---------|------------|
| Best cross-platform image library for .NET | SixLabors.ImageSharp |
| How to extract major colors | ImageSharp Octree quantizer + pixel histogram |
| How to read clipboard images cross-platform | Platform-specific providers (Windows P/Invoke, Linux `xclip`, macOS `pngpaste`) |
| CLI parser | System.CommandLine |
| YAML support | YamlDotNet |
| Color models | RGB default, plus CMYK, HSL, HEX |
| Named colors | CSS Color Module Level 4 list with nearest-neighbor matching |
| Cross-platform release binaries | `dotnet publish` self-contained single-file + GitHub Actions |
| Install mechanism | `install.sh`, `install.ps1`, and manual GitHub Releases download |
