# 2. JSON as the contract between CLI verbs

## Status

Accepted

## Context

The CLI is split into verbs (`scan`, `render`, and `audit` in phase 3) that need to
compose (`scan` produces input for `render` and `audit`) and, later, be consumed by
non-.NET hosts — a VS Code extension (TypeScript) and a Zed extension (Rust/WASM) in
phase 2. Passing in-memory `SolutionView.Core` objects between verbs would only work
within a single process and would tie every future consumer to the .NET runtime.

## Decision

Each verb reads and/or writes a JSON document conforming to
`schema/graph.v1.schema.json`. `scan` writes it; `render` and `audit` read it (and
`audit` writes an updated copy). A `schemaVersion` field on the document tracks breaking
changes to the shape.

## Consequences

- Verbs compose (`scan | render`) without sharing process memory or a common runtime.
- The same `graph.json` is a stable target for the phase 2 editor extensions and any
  future consumer, without them needing to link against .NET.
- Forces the graph shape to be designed deliberately, as a versioned contract, instead of
  being an implicit byproduct of whichever renderer was written first.
- Every shape change is now a compatibility decision (bump `schemaVersion`, update
  `CHANGELOG.md`), not a private refactor internal to one project.
