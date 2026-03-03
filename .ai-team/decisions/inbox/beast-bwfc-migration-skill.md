# Decision: Distributable BWFC Migration Skill

**By:** Beast (Technical Writer)
**Date:** 2026-03-03
**Context:** Jeff pivoted from 9-doc migration toolkit to single Copilot skill file

## What

Created `.github/skills/bwfc-migration/SKILL.md` — a distributable GitHub Copilot skill file designed to be copied into any project's `.github/skills/` folder to teach Copilot how to migrate that project from Web Forms to Blazor using BWFC.

This is DIFFERENT from `.github/skills/webforms-migration/SKILL.md` (internal project skill). The new skill is external-facing and self-contained.

## Key Design Decisions

1. **Single file, not 9 documents.** Jeff explicitly changed direction: "I'd rather this deliver a skill then instructions for the AI agent." All toolkit content consolidated into one SKILL.md with GitHub Copilot skill frontmatter format.

2. **Self-contained / NuGet-first.** Zero references to internal repo paths (`scripts/bwfc-scan.ps1`, `.ai-team/`, `planning-docs/`). BWFC is installed from NuGet (`dotnet add package Fritz.BlazorWebFormsComponents`). The file works when dropped into any project.

3. **Copilot-optimized, not human-optimized.** Tables over prose. Exact code transforms. Literal before/after examples. Written for a Copilot instance that reads instructions literally.

4. **Preserves existing internal skill.** The `webforms-migration/SKILL.md` remains unchanged for internal project use (e.g., WingtipToys migration). The new `bwfc-migration/SKILL.md` is the external-facing version.

5. **Architecture decision templates are new content.** The 10 decision templates (Session→DI, Identity→Blazor Identity, EF6→EF Core, etc.) were synthesized from Forge's ARCHITECTURE-GUIDE design and the migration agent. This is the biggest content addition vs. the internal skill.

6. **Honest about limitations.** Explicitly lists what BWFC does NOT cover: DataSource controls, Wizard, Web Parts, AJAX Toolkit extenders. Provides recommended alternatives for each.

## Why

Jeff reframed the project deliverable: the final product is a migration acceleration system, and the Copilot skill is the primary user-facing interface. A single skill file is more portable, discoverable, and Copilot-native than a folder of markdown documents.

## Impact on Other Agents

- **Forge/Cyclops:** If BWFC components are added/removed or APIs change, the `bwfc-migration` skill needs updating (control translation table, component coverage summary).
- **All:** The `migration-toolkit/` folder documents (README, QUICKSTART, etc.) still exist but are now secondary artifacts. The skill is the primary deliverable.
- **Jubilee:** No sample page changes needed.
