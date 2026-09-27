# 5. Separate SolutionView.Core from every host

## Status

Accepted

## Context

Phase 2 (an LSP host) and phase 3 (`audit`) are both expected to reuse the same
scanning and graph-building logic as phase 1's CLI, just from a different entry point.
If that logic is entangled with `Console.WriteLine` calls or CLI-framework types, every
new host has to unpick that coupling before it can reuse anything, and each host ends up
re-solving the same problem instead of adding a thin layer.

## Decision

Split the solution into `SolutionView.Core` — scanning, the in-memory graph model, ID
assignment, JSON serialization — with zero references to `Console` or any CLI framework;
and `SolutionView.Cli` — argument parsing, verb dispatch, wiring stdin/stdout/files to
Core. Any future host (an LSP server, an audit-only tool) is a new project that
references `SolutionView.Core` and never `SolutionView.Cli`.

## Consequences

- Core is trivially unit-testable without capturing or mocking console output.
- Phase 2/3 hosts are new projects, not refactors of existing ones.
- Enforces the JSON-as-contract boundary (ADR-0002) at the assembly level — Core doesn't
  even have the types needed to bypass it — rather than relying on convention.
- One more project to wire into the solution and CI from day one.
- Requires ongoing discipline: CLI-specific concerns (progress bars, `Spectre.Console`
  types, argument-parsing attributes) must not leak into Core. Worth a passing check in
  code review, since nothing else will catch it.
