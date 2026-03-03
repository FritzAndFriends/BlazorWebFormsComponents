# Decision: Migration Toolkit Content Structure

**By:** Beast (Technical Writer)
**Date:** 2026-03-03
**Context:** Migration toolkit authoring per Forge's MIGRATION-TOOLKIT-DESIGN.md

## What

Created 6 priority documents in `/migration-toolkit/` following Forge's design:
1. README.md (entry point)
2. QUICKSTART.md (step-by-step)
3. CONTROL-COVERAGE.md (52-component table)
4. METHODOLOGY.md (three-layer pipeline)
5. CHECKLIST.md (per-page template)
6. copilot-instructions-template.md (drop-in Copilot config)

## Key Content Decisions

1. **copilot-instructions-template.md is self-contained** — unlike other toolkit docs that use relative links to scripts/skill/agent, this template includes condensed migration rules inline. Reason: developers copy this file into their own project where BWFC relative paths don't exist. It must work standalone.

2. **CONTROL-COVERAGE.md is the single coverage table** — other toolkit docs link to it rather than duplicating the 52-component table. This follows Forge's "no duplication" directive.

3. **Remaining 3 documents deferred** — ARCHITECTURE-GUIDE.md, FAQ.md, and CASE-STUDY.md from the design are not yet written. They are lower priority per Forge's priority ordering. Can be authored in a follow-up.

## Why

Jeff reframed the project as a "migration acceleration system." The toolkit is the user-facing product documentation for that system. These 6 docs cover the critical path from discovery to execution.

## Impact on Other Agents

- **Cyclops/Forge:** If scripts (`bwfc-scan.ps1`, `bwfc-migrate.ps1`) or skill (`SKILL.md`) change parameters or behavior, toolkit docs may need updates (especially QUICKSTART.md and copilot-instructions-template.md).
- **Jubilee:** The QUICKSTART references `samples/AfterWingtipToys/` as reference implementation.
- **All:** Three remaining docs (ARCHITECTURE-GUIDE.md, FAQ.md, CASE-STUDY.md) can be authored when prioritized.
