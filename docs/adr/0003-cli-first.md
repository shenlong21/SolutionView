# 3. Build a plain CLI first; defer an LSP host to phase 2

## Status

Accepted

## Context

Two ways to expose the analysis were considered: a plain CLI, or an LSP server (since
both VS Code and Zed ship LSP clients, and phase 2 wants extensions for both). LSP is
built around live document editing — `textDocument/didOpen`/`didChange`/`didClose`,
per-keystroke diagnostics — while a whole-solution scan is a batch, workspace-level
operation with no natural document lifecycle. Using LSP now would mean using it purely
for its transport and client-scaffolding, not its actual document model. At the current
scale (~25 projects) a fresh scan runs well under a second, so there's no live-editing
use case yet that would justify a persistent server process.

## Decision

Build a plain CLI first, with `scan`/`render`/`audit` verbs. Defer any LSP host to
phase 2, as an additional thin entry point over the same `SolutionView.Core` logic — not
a rewrite of it.

## Consequences

- Simplest thing to build, test, and debug standalone right now.
- Because Core has no CLI-specific coupling (ADR-0005), adding an LSP host later is a new
  project referencing Core, not a restructuring of existing code.
- Phase 1 editor integration is limited to "run the CLI, read its output" — no live
  re-scan-on-save until phase 2 exists.
- If a live-editing use case shows up before phase 2 is planned, this decision should be
  revisited rather than quietly worked around.
