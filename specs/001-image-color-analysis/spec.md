# Feature Specification: Image Color Analysis CLI

**Feature Branch**: `[001-image-color-analysis]`

**Created**: 2026-08-29

**Status**: Draft

**Input**: User description: "Create a cli that takes an image file or memory pasted image and anlayse its major colors. The colors scheme depth can also be taken from input so that a user may want only the classification among RGB or CMYK or other popular spread. Memory pasted image means someone takes a screenshot of UI desktop or uses ctrl-c in an image editor. The output should be in text names by default however input to the cli may ask for visual colors on screen (the closest possible) or even as json / jsonl / yaml"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Analyze an Image File (Priority: P1)

A user has an image saved on disk and wants to know the dominant colors in plain language. They run the command with a file path and receive the top major color by default, including its name and numeric values. They can also request more colors with a count option.

**Why this priority**: File-based analysis is the most common, reliable, and scriptable way to process images. It delivers immediate value without requiring clipboard integration or graphical interaction.

**Independent Test**: This can be fully tested by supplying a known image file and verifying that the returned color names, numeric values, and coverage percentages match expectations.

**Acceptance Scenarios**:

1. **Given** a valid image file path, **When** the user runs the command, **Then** the tool outputs the major colors as text names with numeric values and their relative presence in the image.
2. **Given** a file path that does not exist or is not a supported image, **When** the user runs the command, **Then** the tool reports a clear error and exits with a non-zero status.

---

### User Story 2 - Analyze a Pasted Image from Memory (Priority: P2)

A designer takes a screenshot of a user interface or copies an image in an editor using Ctrl-C, then runs the command with the clipboard option. The tool reads the image from the system clipboard and reports the major colors.

**Why this priority**: Clipboard support removes the need to save temporary files and fits naturally into creative workflows where users copy images directly from design tools or screenshots.

**Independent Test**: This can be fully tested by placing a known image on the clipboard and verifying that the tool produces the same major-color results as when the image is read from a file.

**Acceptance Scenarios**:

1. **Given** a supported image is on the system clipboard, **When** the user runs the command with the clipboard option, **Then** the tool outputs the major colors as text names with numeric values.
2. **Given** the clipboard contains no image data, **When** the user runs the command with the clipboard option, **Then** the tool reports that no image was found and exits with a non-zero status.

---

### User Story 3 - Choose a Color Scheme or Model (Priority: P2)

A user wants color values expressed in a specific color model, such as RGB or CMYK, rather than named colors. They select the desired color scheme through a command option and receive values in that model.

**Why this priority**: Different workflows require different color representations. Designers, printers, and developers each have preferred color models, so configurable output increases the tool's usefulness across disciplines.

**Independent Test**: This can be fully tested by running the command with the same image using different color scheme options and confirming that the output values are converted correctly.

**Acceptance Scenarios**:

1. **Given** a valid image and a selected color model, **When** the user runs the command, **Then** the tool outputs major colors in the requested color model.
2. **Given** an unsupported or misspelled color model, **When** the user runs the command, **Then** the tool lists the supported models and exits with a non-zero status.

---

### User Story 4 - Output Results in Machine-Readable Formats (Priority: P3)

A developer wants to use the color results in a script or pipeline. They request output as JSON, JSONL, or YAML so another program can parse the values directly.

**Why this priority**: Machine-readable output turns the tool into a reusable component in automated workflows, such as build pipelines, design systems, or reporting tools.

**Independent Test**: This can be fully tested by running the command with each supported output format and validating the structure with a standard parser.

**Acceptance Scenarios**:

1. **Given** a valid image and the JSON output option, **When** the user runs the command, **Then** the tool emits a single valid JSON object containing the major colors and their coverage.
2. **Given** a valid image and the JSONL output option, **When** the user runs the command, **Then** the tool emits one color per line as a valid JSON object.
3. **Given** a valid image and the YAML output option, **When** the user runs the command, **Then** the tool emits a valid YAML document containing the major colors and their coverage.

---

### Edge Cases

- **Corrupt or truncated image**: The tool detects that the image cannot be decoded and reports a clear error without crashing.
- **Unsupported image format**: The tool informs the user which formats are accepted.
- **Empty or blank image**: The tool reports that no distinguishable colors were found.
- **Very large image**: The tool completes within a reasonable time and memory budget, or reports that the image exceeds limits rather than failing silently.
- **Clipboard permission denied**: The tool reports that clipboard access is unavailable and suggests using a file path.
- **Named color ambiguity**: When a color does not closely match any named color, the tool reports the closest available name and the actual color values.
- **Visual output unsupported**: The tool reports that the terminal cannot display visual output and exits with a non-zero status.
- **Both file path and standard input present**: The tool uses the file path and ignores standard input.

## Clarifications

### Session 2026-08-29

- **Q:** What open-source license will the getmajorcolors project be released under so it remains compatible with its MIT and Apache-2.0 dependencies? → **A:** MIT License.
- **Q:** Should the CLI read an image from standard input when neither a file path nor the clipboard option is provided? → **A:** Yes, read from standard input when no file path is given.
- **Q:** How should the CLI behave when both a file path and piped standard input are present at the same time? → **A:** Use the file path and ignore standard input.
- **Q:** Should the visual on-screen color output be a built-in default option or an optional extension behavior, and how should unsupported terminals be handled? → **A:** Visual on-screen output is optional and off by default. If the terminal cannot support it, the tool prints a graceful error message and exits with a non-zero status instead of crashing.
- **Q:** Should the default text output include exact numeric color values alongside the color names? → **A:** Yes, the default text output includes color names and numeric values.
- **Q:** Should the tool support a count option to control how many major colors are returned? → **A:** Yes, support a count option with a default of 1.
- **Q:** Which distribution channels should be the primary targets for the first release? → **A:** Linux, Windows, and macOS binaries as equal first-class targets.
- **Q:** Where should the built binaries be published so users can download and install them without compiling from source? → **A:** GitHub Releases with attached binaries and checksums.
- **Q:** Should the release artifacts include code-signed binaries or is checksum verification sufficient for the first release? → **A:** SHA-256 checksums are sufficient for the first release.
- **Q:** Should the project provide an install script, or should users download and place the binary manually? → **A:** Provide a simple install script for each platform, plus manual download instructions as a fallback.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The tool MUST accept an image file path as input.
- **FR-002**: The tool MUST accept an image from the system clipboard as input via an explicit option.
- **FR-003**: The tool MUST accept an image from standard input when no file path or clipboard option is provided.
- **FR-004**: The tool MUST prioritize the file path over standard input when both are present.
- **FR-005**: The tool MUST identify the major colors present in the provided image.
- **FR-006**: The tool MUST report each major color as a human-readable text name by default.
- **FR-007**: The tool MUST include numeric color values alongside names in the default text output.
- **FR-008**: The tool MUST allow the user to choose the color model or scheme used for output, such as RGB or CMYK.
- **FR-009**: The tool MUST support at least the RGB and CMYK color models.
- **FR-010**: The tool MUST support output in plain text, JSON, JSONL, and YAML formats.
- **FR-011**: The tool MUST include the relative presence or coverage percentage of each major color in the output.
- **FR-012**: The tool MUST validate all input and report meaningful errors to standard error.
- **FR-013**: The tool MUST return a non-zero exit code when it cannot complete the requested analysis.
- **FR-014**: The tool MUST allow the user to control how many major colors are reported, defaulting to 1.
- **FR-015**: The tool MUST provide visual on-screen color output only when explicitly requested, and MUST report a graceful error if the terminal cannot support it.
- **FR-016**: The project MUST publish release binaries for Linux, Windows, and macOS via GitHub Releases.
- **FR-017**: The project MUST publish SHA-256 checksums alongside release binaries.
- **FR-018**: The project MUST provide a simple install script for each supported platform, with manual download instructions as a fallback.

### Key Entities *(include if feature involves data)*

- **Image Input**: The source image provided by the user, either as a file path or from the clipboard. It is validated before processing.
- **Major Color**: A dominant color identified in the image. It has a relative presence, a set of values in the selected color model, and optionally a human-readable name.
- **Color Model**: A coordinate system for describing colors, such as RGB or CMYK. The user selects this to control how color values are reported.
- **Output Format**: The presentation style of the results, such as plain text names, JSON, JSONL, or YAML.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can analyze a typical image file and receive results in under 5 seconds on standard hardware.
- **SC-002**: The tool reports major colors with coverage percentages that sum to 100% within a 1% tolerance.
- **SC-003**: The tool supports at least RGB and CMYK color models with correctly converted values.
- **SC-004**: The tool produces valid JSON, JSONL, and YAML output that can be parsed by standard tools.
- **SC-005**: The tool runs on Linux, Windows, and macOS without requiring the user to compile from source.
- **SC-006**: Users receive a clear error message and a non-zero exit code for corrupt, missing, or unsupported inputs.
- **SC-007**: Clipboard-based analysis produces results equivalent to file-based analysis for the same image.
- **SC-008**: Standard-input analysis produces results equivalent to file-based analysis for the same image.
- **SC-009**: Visual output requested on an unsupported terminal produces a clear error message and a non-zero exit code.
- **SC-010**: Every release provides binaries for Linux, Windows, and macOS, each with a matching SHA-256 checksum.
- **SC-011**: Users can install the tool using a provided install script or by downloading a single binary from GitHub Releases.

## Assumptions

- Users will provide images in common raster formats such as PNG, JPEG, GIF, and BMP.
- The default number of major colors reported is 1.
- Named-color output is based on a standard, widely recognized color name list.
- The default color model is RGB unless the user explicitly requests another model.
- Visual output on screen is optional and off by default; it renders simple color swatches when the terminal supports it.
- When a file path and standard input are both available, the file path takes precedence.
- Clipboard access may require platform-specific permissions; when unavailable, the user can always fall back to a file path or standard input.
- The first release targets Linux, Windows, and macOS as equal first-class platforms.
- Release artifacts are published on GitHub Releases with SHA-256 checksums.
- Platform-specific install scripts are provided, with manual download instructions as a fallback.
- The project is released under the MIT License, which is compatible with all planned dependencies.
