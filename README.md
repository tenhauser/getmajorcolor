# getmajorcolors

A cross-platform command-line tool that identifies the major colors in an image.

## Features

- Analyze an image from a file path, the system clipboard, or standard input.
- Output dominant colors as human-readable names with numeric values.
- Choose from RGB, CMYK, HSL, or HEX color models.
- Export results as plain text, JSON, JSONL, YAML, or ANSI truecolor swatches.
- Self-contained binaries for Linux, Windows, and macOS.

## Installation

### Using the install script

**Linux / macOS**

```bash
curl -sSL https://raw.githubusercontent.com/tenhauser/getmajorcolor/main/install.sh | bash
```

**Windows (PowerShell)**

```powershell
Invoke-RestMethod -Uri https://raw.githubusercontent.com/tenhauser/getmajorcolor/main/install.ps1 | Invoke-Expression
```

### Manual

Download the latest binary for your platform from [GitHub Releases](https://github.com/tenhauser/getmajorcolor/releases) and place it on your `PATH`.

## Usage

```bash
# Analyze a file (default: top color, RGB, text)
getmajorcolors photo.jpg

# Top 5 colors in CMYK
getmajorcolors --count 5 --model cmyk --format text photo.jpg

# JSON output
getmajorcolors --count 3 --format json screenshot.png

# Read from clipboard
getmajorcolors --clipboard --count 4

# Read from stdin
curl -s https://example.com/image.png | getmajorcolors --count 3 --format yaml

# Visual swatches
getmajorcolors --visual wallpaper.jpg
```

## Options

| Option | Short | Description |
|--------|-------|-------------|
| `--clipboard` | `-c` | Read image from the system clipboard. |
| `--count` | `-n` | Number of major colors to report (default: 1). |
| `--model` | `-m` | Color model: `rgb`, `cmyk`, `hsl`, `hex` (default: `rgb`). |
| `--format` | `-f` | Output format: `text`, `json`, `jsonl`, `yaml`, `visual` (default: `text`). |
| `--visual` | `-v` | Render ANSI truecolor swatches. |
| `--help` | `-h` | Show help text. |
| `--version` | | Show version. |

## Exit Codes

| Code | Meaning |
|------|---------|
| 0 | Success |
| 1 | Unexpected/internal error |
| 2 | Invalid or missing image file |
| 3 | Clipboard does not contain image data |
| 4 | Standard input empty / no input provided |
| 5 | Unsupported color model |
| 6 | Unsupported output format |
| 7 | Visual output unsupported by terminal |
| 8 | Image corrupt or exceeds size limits |

## Building from Source

```bash
dotnet build
dotnet test
dotnet run --project src/GetMajorColors -- photo.jpg
```

## License

[MIT](LICENSE)

Third-party license notices are available in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
