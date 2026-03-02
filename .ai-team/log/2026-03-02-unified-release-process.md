# Session: 2026-03-02 — Unified Release Process

**Requested by:** Jeffrey T. Fritz

## Who Worked

- **Forge** — Audited all 7 CI/CD workflows, identified 9 problems (2 critical), proposed unified release.yml
- **Cyclops** — Implemented unified release workflow, fixed version.json, converted legacy workflows

## What Was Done

1. Forge audited all 7 CI/CD workflows and identified 9 problems:
   - **P1 (CRITICAL):** version.json/tag mismatch — NBGV computes wrong versions because version.json is out of sync with git tags
   - **P2 (CRITICAL):** Independent triggers — no single event coordinates all release artifacts (NuGet, Docker, docs, demos)
   - P3–P9: Missing GitHub Release automation, deprecated `::set-output`, broken release regex, Docker path filter race condition, unversioned demo artifacts, no version in docs, inconsistent tag history

2. Cyclops implemented the unified release process:
   - **NEW:** `.github/workflows/release.yml` — 5 coordinated jobs (build-and-test, publish-nuget, deploy-docker, deploy-docs, build-demos), triggered by GitHub Release publication
   - **FIX:** `version.json` updated from `"0.17"` to `"0.17.0"` (3-segment SemVer)
   - **MODIFY:** `nuget.yml` — removed tag trigger, added `workflow_dispatch` with version input (emergency escape hatch)
   - **MODIFY:** `deploy-server-side.yml` — removed push trigger + path filters, kept `workflow_dispatch` only
   - **FIX:** `docs.yml` — replaced deprecated `::set-output` with `$GITHUB_OUTPUT`
   - **MODIFY:** `demo.yml` — added NBGV version computation, versioned artifact names

3. PR #408 opened for the changes.

## Decisions Made

- Use 3-segment SemVer (`0.17.0`) for all version references
- GitHub Release publication is the single trigger for all release artifacts
- Existing workflows (nuget.yml, deploy-server-side.yml) kept as `workflow_dispatch`-only emergency escape hatches
- docs.yml retains push-to-main deploy for doc-only changes
- Version extracted from release tag and passed via `-p:Version=` to override NBGV

## Key Outcomes

- All release artifacts (NuGet, Docker, docs, demos) now coordinated through single workflow
- Version drift between artifacts eliminated
- Release process: Create GitHub Release → everything ships automatically
- PR #408 ready for review
