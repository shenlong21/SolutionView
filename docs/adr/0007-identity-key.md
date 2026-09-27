# 7. Deterministic integer IDs for node identity

## Status

Accepted

## Context

Several identity schemes were considered for `projects[].id` / `packages[].id` and the
edges that reference them:

- **Raw MSBuild/`.sln` GUIDs** — unstable across a project being re-added to the
  solution, don't exist for plain SDK-style `.csproj` files without a `.sln` entry,
  aren't shared across two solutions containing the same project, and solution folders
  appear as fake "projects" in a `.sln` that would need filtering out.
- **The project/package name itself** — awkward to reference reliably: names aren't
  guaranteed unique in the way a path is, and using a full path as the key makes edge
  arrays verbose (`{"from": "src/Pramaan.API/Pramaan.API.csproj", ...}` repeated for
  every edge).
- **Arbitrary, non-deterministic incrementing integers** — short and easy to reference,
  but assignment order depends on filesystem/dictionary enumeration order, which isn't
  guaranteed stable. Re-scanning an unchanged solution can reassign different IDs to the
  same projects, which breaks diffing `graph.json` across scans, blocks any future
  incremental-scan feature (which needs to diff old vs. new graphs by ID), and would
  break a future ID-based suppression/baseline file for `audit` findings.

The choice isn't actually "opaque integer vs. readable name" — those are two separate
axes. An integer ID can be made deterministic without becoming a name, by controlling
the order IDs are assigned in.

## Decision

Assign each node a small integer ID, generated deterministically: sort the collection by
its natural stable key before assigning sequential integers — project `path` for
projects, `(name, version)` for packages. Not a raw GUID, not the name/path itself, and
not an arbitrary non-deterministic integer.

## Consequences

- IDs stay short and easy to reference in `dependencies` entries, same as a plain
  incrementing integer would be.
- Re-scanning an unchanged solution reproduces byte-for-byte identical `projects` and
  `packages` arrays (aside from the `generated` timestamp), so diffs across scans stay
  meaningful.
- Lays the groundwork for incremental scanning and ID-based suppression/baseline files in
  phase 3's `audit`, without needing to redesign identity later.
- Requires discipline in `SolutionView.Core`: the assignment step must explicitly sort
  before enumerating, rather than relying on whatever order the filesystem or a
  dictionary happens to produce. Worth a unit test asserting two scans of the same
  unchanged sample solution produce identical IDs.
- IDs are still scan-scoped implementation details, not portable identifiers outside this
  tool's own output — unlike a path, an ID alone means nothing in a bug report without
  the corresponding `graph.json` to cross-reference it against.
