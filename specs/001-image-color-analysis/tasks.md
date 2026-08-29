# Tasks: Image Color Analysis CLI

**Input**: Design documents from `/specs/001-image-color-analysis/`

**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/](contracts/)

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic repository structure

- [x] T001 Create top-level `LICENSE` file with MIT License text in `c:\dev\local\cli\getmajorcolors\LICENSE`
- [x] T002 Create solution file and console project at `src/GetMajorColors/GetMajorColors.csproj` targeting `net8.0`
- [x] T003 Create test project at `tests/GetMajorColors.Tests/GetMajorColors.Tests.csproj` referencing xUnit and the console project
- [x] T004 Add NuGet package references: `SixLabors.ImageSharp`, `System.CommandLine`, `YamlDotNet` in `src/GetMajorColors/GetMajorColors.csproj`
- [x] T005 [P] Create folder structure per plan.md under `src/GetMajorColors/` and `tests/GetMajorColors.Tests/`
- [x] T006 Create initial README with build/run instructions and install script usage in `c:\dev\local\cli\getmajorcolors\README.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core abstractions and shared services that all user stories depend on

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T007 Create `ColorModel` enum (Rgb, Cmyk, Hsl, Hex) in `src/GetMajorColors/Models/ColorModel.cs`
- [x] T008 Create `OutputFormat` enum (Text, Json, Jsonl, Yaml, Visual) in `src/GetMajorColors/Models/OutputFormat.cs`
- [x] T009 Create `MajorColor` record and `ColorValues` record in `src/GetMajorColors/Models/MajorColor.cs`
- [x] T010 Create `ImageSourceKind` enum and `ImageInput` record in `src/GetMajorColors/Models/ImageInput.cs`
- [x] T011 Implement `IColorNameResolver` and `CssColorNameResolver` in `src/GetMajorColors/Services/CssColorNameResolver.cs`
- [x] T012 Implement RGB/CMYK/HSL/HEX conversion helpers in `src/GetMajorColors/Services/ColorConverter.cs`
- [x] T013 Implement `IColorAnalyzer` and `ImageSharpColorAnalyzer` in `src/GetMajorColors/Services/ImageSharpColorAnalyzer.cs`
- [x] T014 Define `IImageSource`, `IOutputWriter`, and `IClipboardImageProvider` interfaces in `src/GetMajorColors/Services/`
- [x] T015 Implement `FileImageSource` and `StdinImageSource` in `src/GetMajorColors/Services/`
- [x] T016 Implement output writers: `TextOutputWriter`, `JsonOutputWriter`, `JsonlOutputWriter`, `YamlOutputWriter`, `VisualOutputWriter` in `src/GetMajorColors/Output/`
- [x] T017 Create sample test images in `tests/GetMajorColors.Tests/TestImages/`
- [x] T018 Add unit tests for color conversion and named-color resolution in `tests/GetMajorColors.Tests/Unit/`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Analyze an Image File (Priority: P1) 🎯 MVP

**Goal**: User can pass a file path and receive the top major color(s) as named colors with numeric values and coverage.

**Independent Test**: Run `dotnet run --project src/GetMajorColors -- tests/GetMajorColors.Tests/TestImages/solid-red-100x100.png` and verify output names a red color with RGB near `(255, 0, 0)` and coverage near `100%`.

### Tests for User Story 1

- [x] T019 [P] [US1] Add integration test for valid image file input in `tests/GetMajorColors.Tests/Integration/FileInputTests.cs`
- [x] T020 [P] [US1] Add integration test for missing/unsupported file path in `tests/GetMajorColors.Tests/Integration/FileInputTests.cs`
- [x] T021 [P] [US1] Add output writer contract tests for text output in `tests/GetMajorColors.Tests/Unit/OutputWriterTests.cs`

### Implementation for User Story 1

- [x] T022 [US1] Implement root command and option parsing in `src/GetMajorColors/Program.cs`
- [x] T023 [US1] Implement `AnalyzeCommand` handler wiring file path → analyzer → output writer in `src/GetMajorColors/Commands/AnalyzeCommand.cs`
- [x] T024 [US1] Add input validation and meaningful stderr error messages for file path errors
- [x] T025 [US1] Implement default text output formatter with name, numeric values, and coverage percentage
- [x] T026 [US1] Add exit-code mapping for success and file-related failures

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Analyze a Pasted Image from Memory (Priority: P2)

**Goal**: User can pass `--clipboard` to analyze an image stored on the system clipboard.

**Independent Test**: Place a known image on the clipboard, run `dotnet run --project src/GetMajorColors -- --clipboard`, and verify results match the file-based output for the same image.

### Tests for User Story 2

- [x] T027 [P] [US2] Add unit tests for clipboard provider selection logic by OS in `tests/GetMajorColors.Tests/Unit/ClipboardProviderTests.cs`
- [x] T028 [P] [US2] Add integration test for clipboard input using a stub provider in `tests/GetMajorColors.Tests/Integration/ClipboardInputTests.cs`

### Implementation for User Story 2

- [x] T029 [P] [US2] Implement `ClipboardImageSource` in `src/GetMajorColors/Services/ClipboardImageSource.cs`
- [x] T030 [P] [US2] Implement `WindowsClipboardImageProvider` using Windows Clipboard API in `src/GetMajorColors/Clipboard/WindowsClipboardImageProvider.cs`
- [x] T031 [P] [US2] Implement `LinuxClipboardImageProvider` shelling out to `xclip` in `src/GetMajorColors/Clipboard/LinuxClipboardImageProvider.cs`
- [x] T032 [P] [US2] Implement `MacOSClipboardImageProvider` shelling out to `pngpaste` in `src/GetMajorColors/Clipboard/MacOSClipboardImageProvider.cs`
- [x] T033 [US2] Wire `--clipboard` option into `AnalyzeCommand` input source selection
- [x] T034 [US2] Add clipboard permission/no-image error handling with exit code 3

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Choose a Color Scheme or Model (Priority: P2)

**Goal**: User can select a color model (`--model rgb|cmyk|hsl|hex`) and receive numeric values in that model.

**Independent Test**: Run the same image with `--model rgb` and `--model cmyk` and verify the numeric values are correctly converted.

### Tests for User Story 3

- [x] T035 [P] [US3] Add unit tests for CMYK conversion in `tests/GetMajorColors.Tests/Unit/ColorConversionTests.cs`
- [x] T036 [P] [US3] Add unit tests for HSL and HEX conversion in `tests/GetMajorColors.Tests/Unit/ColorConversionTests.cs`
- [x] T037 [P] [US3] Add integration test for unsupported color model in `tests/GetMajorColors.Tests/Integration/ModelOptionTests.cs`

### Implementation for User Story 3

- [x] T038 [US3] Add `--model` option to root command in `src/GetMajorColors/Options/OutputOptions.cs`
- [x] T039 [US3] Wire selected `ColorModel` through `AnalyzeCommand` into `ColorValues` production
- [x] T040 [US3] Implement `ColorValues` formatting for CMYK, HSL, and HEX in output writers
- [x] T041 [US3] Add validation that lists supported models and returns exit code 5 for unsupported models

**Checkpoint**: User Stories 1, 2, and 3 should all work independently

---

## Phase 6: User Story 4 - Output Results in Machine-Readable Formats (Priority: P3)

**Goal**: User can request output as JSON, JSONL, or YAML.

**Independent Test**: Run `getmajorcolors -f json`, `-f jsonl`, and `-f yaml` against a sample image and validate each with standard parsers.

### Tests for User Story 4

- [x] T042 [P] [US4] Add contract tests for JSON output schema in `tests/GetMajorColors.Tests/Contract/JsonOutputContractTests.cs`
- [x] T043 [P] [US4] Add contract tests for JSONL output schema in `tests/GetMajorColors.Tests/Contract/JsonlOutputContractTests.cs`
- [x] T044 [P] [US4] Add contract tests for YAML output schema in `tests/GetMajorColors.Tests/Contract/YamlOutputContractTests.cs`

### Implementation for User Story 4

- [x] T045 [US4] Add `--format` option to root command in `src/GetMajorColors/Options/OutputOptions.cs`
- [x] T046 [P] [US4] Implement `JsonOutputWriter` using `System.Text.Json` in `src/GetMajorColors/Output/JsonOutputWriter.cs`
- [x] T047 [P] [US4] Implement `JsonlOutputWriter` using `System.Text.Json` in `src/GetMajorColors/Output/JsonlOutputWriter.cs`
- [x] T048 [P] [US4] Implement `YamlOutputWriter` using `YamlDotNet` in `src/GetMajorColors/Output/YamlOutputWriter.cs`
- [x] T049 [US4] Add output format selection in `AnalyzeCommand`
- [x] T050 [US4] Add validation that lists supported formats and returns exit code 6 for unsupported formats

**Checkpoint**: All user stories should now be independently functional

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories and prepare for release

- [x] T051 [P] Implement `--visual` swatch output using ANSI truecolor in `src/GetMajorColors/Output/VisualOutputWriter.cs`
- [x] T052 Add terminal capability detection and graceful exit code 7 when visual output is unsupported
- [x] T053 Add `--count` option with default `1` and validate positive integer input in `src/GetMajorColors/Options/OutputOptions.cs`
- [x] T054 Add `--version` and `--help` handling with usage examples
- [x] T055 Add `--stdin` explicit alias/behavior documentation; ensure stdin is used when no file or clipboard is provided
- [x] T056 Enforce input size limits and maximum image dimensions for safety in `ImageSharpColorAnalyzer` and image sources
- [x] T057 Collect third-party license notices and include them in release archives
- [x] T058 Create `install.sh` for Linux/macOS in `c:\dev\local\cli\getmajorcolors\install.sh`
- [x] T059 Create `install.ps1` for Windows in `c:\dev\local\cli\getmajorcolors\install.ps1`
- [x] T060 Create GitHub Actions release workflow in `.github/workflows/release.yml` building win-x64, linux-x64, osx-x64 binaries with SHA-256 checksums
- [x] T061 [P] Add end-to-end quickstart validation tests from `quickstart.md` in `tests/GetMajorColors.Tests/Integration/QuickstartTests.cs`
- [x] T062 Update README with final usage examples, error codes, and install instructions
- [x] T063 Run full test suite and quickstart validation; fix any regressions

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-6)**: All depend on Foundational phase completion
  - User stories can proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - Uses shared `IImageSource` abstraction; independently testable
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) - Builds on `ColorModel` and output formatting; independently testable
- **User Story 4 (P3)**: Can start after Foundational (Phase 2) - Builds on `OutputFormat` and output writers; independently testable

### Within Each User Story

- Tests (if included) MUST be written and FAIL before implementation
- Models before services
- Services before command handlers
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- All tests for a user story marked [P] can run in parallel
- Models and output writers within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together:
Task: "Add integration test for valid image file input in tests/GetMajorColors.Tests/Integration/FileInputTests.cs"
Task: "Add integration test for missing/unsupported file path in tests/GetMajorColors.Tests/Integration/FileInputTests.cs"
Task: "Add output writer contract tests for text output in tests/GetMajorColors.Tests/Unit/OutputWriterTests.cs"

# Launch models and services for User Story 1 together:
Task: "Implement root command and option parsing in src/GetMajorColors/Program.cs"
Task: "Implement AnalyzeCommand handler wiring file path → analyzer → output writer in src/GetMajorColors/Commands/AnalyzeCommand.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo
5. Add User Story 4 → Test independently → Deploy/Demo
6. Complete Polish phase → Release

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
   - Developer D: User Story 4
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
