# Roadmap

Phase boundaries are written down here on purpose — several ideas (package dependencies,
CVE scanning) have already drifted toward the wrong phase during design discussions.
If it's not listed under a phase's scope, it doesn't belong in that phase's PRs.

## Phase 1 — CLI (current)

**Scope**
- Scan a `.sln` (or a single `.csproj`) via Buildalyzer: projects, direct
  `ProjectReference`s, direct `PackageReference`s.
- Emit `schemaVersion`-tagged JSON conforming to `schema/graph.v1.schema.json` (`scan`).
- Render that JSON to Markdown and to Mermaid-embedded Markdown (`render`).
- Terminal tree view via Spectre.Console.
- Filters: `--exclude-test-projects`, glob excludes.
- Integration tests against `/samples/SampleSolution` — a real `.sln` and real `.csproj`
  fixtures, not hand-typed JSON.

**Non-goals for this phase**
- No transitive package dependencies (ADR-0006).
- No vulnerability data of any kind.
- No interactive visualization.
- Does not modify project files; is not a build tool.

## Phase 2 — Editor integration

**Scope**
- LSP host (`SolutionView.Lsp`) wrapping `SolutionView.Core`, exposing a custom method
  (e.g. `solutionview/getDependencyGraph`) over the standard LSP lifecycle (ADR-0003).
- Thin VS Code extension (`vscode-languageclient`) rendering the graph in a
  TreeView/webview.
- Thin Zed extension (`zed_extension_api`, Rust/WASM) doing the same via Zed's LSP client.

**Non-goals for this phase**
- No reimplementation of scanning logic in TypeScript or Rust — extensions are thin
  clients that call the same Core logic through the LSP host.
- No live re-scan-on-save unless it turns out to be cheap and clearly wanted; not assumed
  up front.

## Phase 3 — Visualization + vulnerability audit

**Scope**
- `audit` verb: enrich `graph.json` with vulnerability data via
  `dotnet list package --vulnerable` or OSV.dev (ADR-0004), keyed by package node ID,
  additive rather than mutating existing package nodes.
- Offline mode and local caching for vulnerability lookups (`--offline` flag), since
  `audit` is the first command in this project that makes a network call.
- Interactive graph viewer: pan/zoom, collapse/expand, direct-vs-transitive filtering.
  Treat this as its own real chunk of work — comparable in size to phases 1 and 2
  combined, not a bonus round tacked on at the end.

**Open dependency on phase 1**
- `dotnet list package --vulnerable --include-transitive` will report vulnerabilities in
  transitive packages, but v1's graph (ADR-0006) has no node for them. Until that's
  revisited, `audit` can flag "this project has an indirect vulnerable dependency"
  without being able to say which direct package pulled it in.
