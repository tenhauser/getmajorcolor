# Quickstart: Image Color Analysis CLI

This guide validates the Image Color Analysis CLI end-to-end after implementation. It references the command contract in [contracts/cli-commands.md](contracts/cli-commands.md) and the data model in [data-model.md](data-model.md).

## Prerequisites

- .NET 8 SDK (for development and local running only; release binaries are self-contained).
- A terminal that supports ANSI truecolor if testing visual output.
- For clipboard tests: an image in the system clipboard and, on Linux, `xclip` installed.

## Build and Run Locally

```bash
# From the repository root
dotnet build src/GetMajorColors/GetMajorColors.csproj
dotnet run --project src/GetMajorColors -- tests/GetMajorColors.Tests/TestImages/solid-red-100x100.png
```

Expected: one line naming a red color with RGB values near `(255, 0, 0)` and coverage near `100%`.

## Validate File Input

```bash
dotnet run --project src/GetMajorColors -- --count 3 --model rgb --format text \
  tests/GetMajorColors.Tests/TestImages/gradient-rgb-200x200.png
```

Expected: up to three colors with percentages that sum to approximately 100%.

## Validate Standard Input

```bash
cat tests/GetMajorColors.Tests/TestImages/solid-red-100x100.png \
  | dotnet run --project src/GetMajorColors -- --count 1 --format json
```

Expected: a single valid JSON object containing one color with `name`, `coverage`, `model`, and `values`.

## Validate JSONL Output

```bash
dotnet run --project src/GetMajorColors -- --count 2 --format jsonl photo-sample.jpg
```

Expected: two lines of JSON, each parseable independently.

## Validate YAML Output

```bash
dotnet run --project src/GetMajorColors -- --count 2 --format yaml photo-sample.jpg
```

Expected: a valid YAML document with a top-level `colors` list.

## Validate Color Model Conversion

```bash
dotnet run --project src/GetMajorColors -- --count 1 --model cmyk --format json \
  tests/GetMajorColors.Tests/TestImages/solid-red-100x100.png
```

Expected: `values` contains `c`, `m`, `y`, `k` keys with CMYK coordinates for red.

## Validate Visual Output

```bash
dotnet run --project src/GetMajorColors -- --visual photo-sample.jpg
```

Expected: colored blocks rendered in the terminal. If the terminal does not support truecolor, a graceful error and exit code 7.

## Validate Clipboard Input

Copy an image to the system clipboard, then run:

```bash
dotnet run --project src/GetMajorColors -- --clipboard --count 3
```

Expected: same results as running the same image from a file path.

## Validate Error Handling

```bash
# Missing file
dotnet run --project src/GetMajorColors -- /does/not/exist.png
echo $?   # Expected: 2

# Unsupported model
dotnet run --project src/GetMajorColors -- --model xyz photo-sample.jpg
echo $?   # Expected: 5

# Empty stdin
echo -n "" | dotnet run --project src/GetMajorColors --
echo $?   # Expected: 4
```

## Run the Test Suite

```bash
dotnet test
```

Expected: all unit and integration tests pass.

## Build Release Binaries

```bash
dotnet publish src/GetMajorColors -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Repeat for `linux-x64` and `osx-x64`. Verify the produced binary runs without the .NET runtime installed, and confirm the release archive contains the `LICENSE` file and any required third-party notices.

## Install from GitHub Release

### Linux / macOS

```bash
curl -sSL https://raw.githubusercontent.com/tenhauser/getmajorcolor/main/install.sh | bash
```

### Windows (PowerShell)

```powershell
Invoke-RestMethod -Uri https://raw.githubusercontent.com/tenhauser/getmajorcolor/main/install.ps1 | Invoke-Expression
```

Expected: the `getmajorcolors` binary is placed on `PATH` and `getmajorcolors --version` prints the release version.
