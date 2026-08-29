<!--
Sync Impact Report
- Version change: 0.0.0 → 1.0.0
- Modified principles: All five placeholder principles replaced with project-specific principles.
- Added sections: Technology Stack & Distribution, Development Workflow & Quality Gates.
- Removed sections: None.
- Follow-up TODOs: None.
-->

# getmajorcolors Constitution

## Core Principles

### I. Cross-Platform Compatibility

The application MUST build and run on Linux without vendor-specific dependencies.
Release artifacts MUST be statically linked or self-contained binaries so users can
install and run the tool without compiling from source. Container-based or
package-manager distribution is secondary; portable binaries are the primary
compatibility target.

**Rationale**: The user base depends on easily available binaries for work. A single
static binary removes friction across distributions and CI environments.

### II. Simplicity

Code and identifiers MUST be simple, explicit, and conventional. Avoid clever
abstractions, deep inheritance, or novel naming schemes. Functions and modules
MUST have one clear responsibility. Configuration surface MUST stay minimal.

**Rationale**: Simple code is easier to audit, document, port, and maintain. It also
reduces the attack surface and the chance of subtle bugs.

### III. Complete Documentation

Every public module, function, type, and command-line option MUST be documented.
README and usage examples MUST cover installation, common invocations, and error
codes. Inline comments MUST explain why non-obvious decisions were made, not what
the code does.

**Rationale**: A CLI tool is used by people who read docs before reading source.
Complete documentation is a core deliverable, not an afterthought.

### IV. Memory Safety and Exploit Resistance

The implementation MUST use a memory-safe language or runtime by default. If
memory-unsafe code is unavoidable, it MUST be isolated, minimized, and reviewed.
All input parsing, image decoding, and file I/O MUST validate sizes and bounds
before allocation or copy operations. Fuzzing or static analysis SHOULD be used
to catch buffer overflows, use-after-free, and integer overflow bugs.

**Rationale**: Image classifiers process untrusted binary input. Memory corruption
bugs are a critical class of exploit that this project refuses to tolerate.

### V. CLI-First Design

All functionality MUST be accessible from the command line. Input MUST be accepted
from file paths or standard input; output MUST go to standard output, with errors
to standard error. The tool MUST return meaningful exit codes and support a
machine-readable output format.

**Rationale**: The application is a CLI image classifier. Scriptability and
predictable I/O are its primary interface contracts.

## Technology Stack & Distribution

- The default implementation language SHOULD be one that produces native,
  memory-safe binaries with minimal runtime dependencies.
- Build scripts MUST produce Linux binaries as part of every release.
- Release artifacts MUST be published alongside checksums and installation
  instructions.
- Dependency choices MUST favor widely available, well-maintained libraries over
  bleeding-edge or niche alternatives.

## Development Workflow & Quality Gates

- Every change MUST pass the existing automated test suite before merge.
- New features MUST include tests and documentation updates in the same change.
- Code review MUST verify adherence to the Core Principles, especially memory
  safety, simplicity, and documentation completeness.
- Releases MUST be tagged with semantic versions and accompanied by release notes.

## Governance

This constitution is the highest authority for project decisions. Amendments MAY
add, remove, or refine principles and workflow rules. Any amendment MUST update
the version line and the Sync Impact Report at the top of this file.

- **Versioning**: MAJOR.MINOR.PATCH. Bump MAJOR for incompatible governance
  removals or redefinitions, MINOR for new principles or materially expanded
  guidance, PATCH for clarifications and wording fixes.
- **Compliance**: All contributions SHOULD be reviewed against this constitution.
- **Amendments**: Proposed amendments MUST be recorded in project history with a
  rationale before the version line is updated.

**Version**: 1.0.0 | **Ratified**: 2026-08-29 | **Last Amended**: 2026-08-29
