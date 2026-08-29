# Output Format Contract

All output is written to standard output. Errors are written to standard error. Numeric values use invariant culture (`.` as decimal separator).

## Text Format (default)

One color per line. Coverage is shown as a percentage.

```text
Crimson #DC143C rgb(220, 20, 60) 45.2%
White #FFFFFF rgb(255, 255, 255) 32.8%
Black #000000 rgb(0, 0, 0) 22.0%
```

When `--model` is not RGB, the numeric values are shown in the requested model:

```text
Crimson cmyk(0, 91, 73, 14) 45.2%
```

## JSON Format

Single JSON object.

```json
{
  "colors": [
    {
      "name": "Crimson",
      "coverage": 0.452,
      "model": "rgb",
      "values": { "r": 220, "g": 20, "b": 60 }
    },
    {
      "name": "White",
      "coverage": 0.328,
      "model": "rgb",
      "values": { "r": 255, "g": 255, "b": 255 }
    }
  ]
}
```

## JSONL Format

One JSON object per line, one per color.

```jsonl
{"name":"Crimson","coverage":0.452,"model":"rgb","values":{"r":220,"g":20,"b":60}}
{"name":"White","coverage":0.328,"model":"rgb","values":{"r":255,"g":255,"b":255}}
```

## YAML Format

```yaml
colors:
  - name: Crimson
    coverage: 0.452
    model: rgb
    values:
      r: 220
      g: 20
      b: 60
  - name: White
    coverage: 0.328
    model: rgb
    values:
      r: 255
      g: 255
      b: 255
```

## Visual Format

Renders swatches using ANSI 24-bit background color escape sequences. Each line contains a colored block followed by the name, hex value, and coverage.

```text
██ Crimson #DC143C 45.2%
██ White  #FFFFFF 32.8%
██ Black  #000000 22.0%
```

If the terminal does not support truecolor, or `NO_COLOR` is set, the tool exits with code 7 and prints a graceful error to stderr.

## Coverage Rules

- Coverage is expressed as a ratio in JSON/JSONL/YAML and as a percentage in text/visual.
- The sum of reported coverages must be within `1%` of `100%`.
- If fewer distinguishable colors exist than requested via `--count`, only the available colors are reported and the tool exits successfully.
