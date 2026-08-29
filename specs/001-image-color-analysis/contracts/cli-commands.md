# CLI Command Contract

## Command Name

`getmajorcolors` (executable name; may be invoked as `getmajorcolors` or `./getmajorcolors` depending on install).

## Synopsis

```text
getmajorcolors [options] [<image-file>]
getmajorcolors [options] --clipboard
```

## Positional Argument

| Argument | Required | Description |
|----------|----------|-------------|
| `<image-file>` | No | Path to a raster image file. If provided, takes precedence over standard input. |

## Options

| Option | Short | Type | Default | Description |
|--------|-------|------|---------|-------------|
| `--clipboard` | `-c` | flag | false | Read the image from the system clipboard instead of a file or stdin. |
| `--count` | `-n` | int | 1 | Number of major colors to report. Must be >= 1. |
| `--model` | `-m` | string | `rgb` | Color model for numeric values: `rgb`, `cmyk`, `hsl`, `hex`. |
| `--format` | `-f` | string | `text` | Output format: `text`, `json`, `jsonl`, `yaml`, `visual`. |
| `--visual` | `-v` | flag | false | Render ANSI truecolor swatches (alias for `--format visual`). |
| `--help` | `-h` | flag | - | Show help text and exit. |
| `--version` | - | flag | - | Show version and exit. |

## Input Precedence

1. If `<image-file>` is provided, use it and ignore standard input.
2. Else if `--clipboard` is provided, read from the clipboard.
3. Else read from standard input.

## Exit Codes

| Code | Meaning |
|------|---------|
| 0 | Success. |
| 1 | Unexpected/internal error. |
| 2 | Invalid or missing image file, or unsupported image format. |
| 3 | Clipboard does not contain image data, or clipboard access failed. |
| 4 | Standard input empty and no other input source provided. |
| 5 | Unsupported color model. |
| 6 | Unsupported output format. |
| 7 | Visual output requested but terminal does not support it. |
| 8 | Image corrupt, truncated, or exceeds size/dimension limits. |

## Environment Variables

| Variable | Effect |
|----------|--------|
| `NO_COLOR` | If set to any non-empty value, disables ANSI color output even for visual/text formats. |

## Examples

```bash
# Default: top color as text
getmajorcolors photo.jpg

# Top 5 colors in CMYK
getmajorcolors --count 5 --model cmyk --format text photo.jpg

# JSON output of 3 RGB colors
getmajorcolors -n 3 -m rgb -f json screenshot.png

# Read from clipboard
getmajorcolors --clipboard -n 4

# Read from stdin
curl -s https://example.com/image.png | getmajorcolors -n 3 -f yaml

# Visual swatches
getmajorcolors --visual wallpaper.jpg
```
