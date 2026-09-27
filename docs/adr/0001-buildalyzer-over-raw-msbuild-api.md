# 1. Use Buildalyzer instead of raw MSBuild APIs

## Status

Accepted

## Context

`scan` needs to extract `ProjectReference`s and direct `PackageReference`s from each
`.csproj` without invoking a full build. Calling the raw `Microsoft.Build` APIs directly
means handling SDK resolution and target evaluation order ourselves, and they are poorly
documented for an "inspect, don't build" use case — most guidance assumes you want to
actually compile.

Buildalyzer wraps MSBuild specifically for static analysis: it runs a design-time build
(evaluation of properties and items) rather than a full compile, which is exactly the
"inspect, don't build" shape this tool needs.

## Decision

Use Buildalyzer to drive project evaluation instead of calling `Microsoft.Build` directly.

## Consequences

- Handles multi-targeting, `Directory.Build.props` inheritance, and SDK-style project
  quirks correctly, instead of us reimplementing MSBuild's resolution logic.
- Meaningfully less code to write and maintain than hand-rolling MSBuild evaluation.
- Adds a third-party dependency; tied to Buildalyzer keeping pace with new SDK/TFM
  versions.
- Slower startup than hand-parsed XML would be — acceptable at this project's scale
  (~25 projects).
- Buildalyzer's `PackageReference` data is direct-only unless the solution has been
  restored (transitive packages require `project.assets.json`). Not a concern for v1,
  which is scoped to direct dependencies only (ADR-0006) — but relevant if that scope
  ever expands.
