### 2026-03-03: User directive — distributable assets location
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Distributable migration skills and PowerShell scripts must NOT live in `.github/skills/`. They belong in a `migration-toolkit/` folder that contains everything needed for external projects to consume. The `.github/skills/` folder is reserved for internal project skills only. Move the 3 BWFC migration skills and the PowerShell scripts (bwfc-scan.ps1, bwfc-migrate.ps1) into migration-toolkit/.
**Why:** User request — the toolkit is a product to distribute, not internal project configuration.
