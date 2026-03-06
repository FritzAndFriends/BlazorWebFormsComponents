### 2026-03-06: User directive — migration-toolkit is end-user distributable
**By:** Jeffrey T. Fritz (via Copilot)
**What:** The `migration-toolkit/` folder is the output distributable included with the NuGet package for end-users. Migration skills belong in `migration-toolkit/skills/`, NOT in `.ai-team/skills/`.
**Why:** User request — clarifies that migration-toolkit is a product artifact, not internal tooling. Skills there serve end-users, not AI team development.
