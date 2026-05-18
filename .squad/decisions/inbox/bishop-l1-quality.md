# Bishop L1 quality fixes

- **Date:** 2026-05-17T13:32:41-04:00
- **Owner:** Bishop

## Decision
For EDMX-backed migrations, the CLI must treat the converter output as the single source of truth for model artifacts on each run.

That means:
1. normalize excluded source-file paths to full paths before `SourceFileCopier` compares them,
2. exclude any source files whose filenames collide with the EF Core files generated from the EDMX, and
3. delete stale excluded output artifacts (for example `Model1.Context.cs`) when rerunning into an existing `After*` folder.

For malformed legacy markup, the late cleanup pass must auto-close unbalanced child tags before a parent/container closes so emitted `.razor` stays syntactically valid even when the source `.aspx` was lax.

## Why
ContosoUniversity Run 28/29 style reruns can look "fixed" in logs while still leaving duplicate EF artifacts or broken Razor in the output tree because the pipeline never cleaned up stale files or balanced malformed containers. Treating generated EDMX output as authoritative and rebalancing markup in the final cleanup pass makes Layer 1 output much more benchmark-runnable without hand edits.
