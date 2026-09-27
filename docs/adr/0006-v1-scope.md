# 6. Scope v1 to direct dependencies only

## Status

Accepted

## Context

Earlier schema drafts modeled package-to-package transitive dependencies (a
`transitiveDependencies`/`directDependencies` list on each package node), so the full
dependency closure for a project could be computed by walking outward from its direct
packages. That turned out to add real complexity for v1: the field naming was easy to
get wrong (see the "does it hold direct or transitive data" confusion during schema
review), a package's dependency set can vary per target framework, and getting accurate
transitive data out of Buildalyzer requires the solution to have been restored first
(`project.assets.json`) — see the caveat in ADR-0001. None of that complexity is needed
to answer this tool's core question: how are the 25 projects in this solution connected,
and what does each one directly pull in.

## Decision

The v1 graph (`schema/graph.v1.schema.json`) includes projects, each project's direct
`ProjectReference`s, and each project's direct `PackageReference`s only. No
package-to-package edges, no transitive closure, are represented in v1.

## Consequences

- Smaller, simpler schema — no ambiguity about what a package's dependency list means,
  because packages don't carry one.
- `scan` works against an unrestored solution, since direct `PackageReference` data is
  exactly what Buildalyzer gives without needing `project.assets.json` (ADR-0001).
- Phase 3's `audit` (`dotnet list package --vulnerable --include-transitive`, ADR-0004)
  will surface vulnerabilities in transitive packages that this graph has no node for.
  Until this ADR is revisited, `audit` can flag a project as having a vulnerable
  transitive dependency but can't pinpoint which direct package pulled it in.
- Adding transitive packages back later is a breaking shape change — it requires a
  `schemaVersion` bump, not a quiet extension.
