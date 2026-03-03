# Decision: Restructure migration-toolkit to be self-contained distribution package

**Author:** Forge
**Date:** 2026-03-03
**Status:** Implemented
**Directive from:** Jeffrey T. Fritz

## Context

Distributable migration skills (bwfc-migration, bwfc-identity-migration, bwfc-data-migration) were located in `.github/skills/` alongside internal project skills (webforms-migration, documentation, component-development, bunit-test-migration, aspire). This mixed distributable assets with internal tooling, making it unclear what end-users should copy to their own projects.

## Decision

Move all distributable migration assets into `migration-toolkit/` as a self-contained distribution package:

- **`migration-toolkit/skills/`** — 3 Copilot skill files that users copy into their project's `.github/skills/`
- **`migration-toolkit/scripts/`** — 2 PowerShell scripts (bwfc-scan.ps1, bwfc-migrate.ps1) that users copy to their project root
- **`.github/skills/`** — retains only internal project skills used by contributors to this repo

Scripts are copied (not moved) because `scripts/` originals are still used by the project internally.

## Consequences

- `migration-toolkit/` is now the single artifact a user downloads/copies to migrate their Web Forms app
- No confusion about which skills are for end-users vs which are internal
- README.md updated with explicit usage instructions and NuGet link
- Relative links in README.md updated from `../scripts/` and `../.github/skills/` to local `scripts/` and `skills/` paths
