### 2026-03-02: Unified Release Process Implementation
**By:** Cyclops
**Status:** Implemented on branch `ci/unified-release-process`

---

## Decision

Implemented the unified release process as designed by Forge and approved by Jeff. All release artifacts (NuGet, Docker, docs, demos) are now coordinated through a single `release.yml` workflow triggered by GitHub Release publication.

## Changes Made

| File | Change |
|------|--------|
| `version.json` | `"version": "0.17"` → `"version": "0.17.0"` (3-segment SemVer) |
| `.github/workflows/release.yml` | **NEW** — unified release workflow with 5 jobs: build-and-test, publish-nuget, deploy-docker, deploy-docs, build-demos |
| `.github/workflows/deploy-server-side.yml` | Removed push trigger + path filters, kept `workflow_dispatch` only |
| `.github/workflows/nuget.yml` | Removed tag trigger, added `workflow_dispatch` with version input for manual emergency republish |
| `.github/workflows/docs.yml` | Fixed deprecated `::set-output` → `$GITHUB_OUTPUT`. Kept push-to-main deploy for doc-only changes. |
| `.github/workflows/demo.yml` | Added NBGV version computation, versioned artifact names |

## Key Design Decisions

1. **Version from release tag, not NBGV:** release.yml extracts version from `github.event.release.tag_name` and passes it explicitly via `-p:Version=` and `-p:PackageVersion=`. This overrides NBGV to guarantee all artifacts match the tag exactly.

2. **Existing workflows kept as escape hatches:** deploy-server-side.yml and nuget.yml are now `workflow_dispatch` only — manual emergency paths that don't interfere with the release workflow.

3. **docs.yml still deploys on push to main:** Doc-only changes (typo fixes, new guides) deploy without waiting for a release. Production docs deploy happens both on push-to-main AND via release.yml.

4. **Fan-out job structure:** build-and-test gates all other jobs. publish-nuget, deploy-docker, deploy-docs, and build-demos run in parallel after tests pass.

## What Team Members Should Know

- **To release:** Create a GitHub Release with tag `v0.17.0` targeting main. Everything else is automatic.
- **Emergency Docker deploy:** Use `workflow_dispatch` on deploy-server-side.yml.
- **Emergency NuGet publish:** Use `workflow_dispatch` on nuget.yml with explicit version input.
- **After each release:** Bump `version.json` on dev to the next minor (e.g., `0.18.0`).
