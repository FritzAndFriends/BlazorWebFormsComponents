# Decision: Health Snapshot in CI Pipeline

**Date:** 2025-07-17
**Author:** Cyclops (Component Dev)
**Status:** Implemented

## Context
The `scripts/GenerateHealthSnapshot` console app can produce a `health-snapshot.json` summarizing component health. Running this in CI ensures every build produces an up-to-date snapshot without manual effort.

## Decision
Added three steps to `build.yml` after the build phase and before tests:
1. Restore the snapshot tool
2. Run it with `--configuration Release` passing workspace root and output path
3. Upload `health-snapshot.json` as a build artifact (`if: always()`)

Also added the snapshot tool to the initial dependency restore block.

## Rationale
- Placing after library build guarantees the ProjectReference is satisfied
- `if: always()` on upload ensures the artifact is captured even on test failures
- Separate restore step keeps the `--no-restore` pattern consistent for the run step
