# BWFC Migration Toolkit

**Migrate your ASP.NET Web Forms application to Blazor — systematically, not heroically.**

This toolkit packages everything you need to take a Web Forms app and bring it to Blazor using the [BlazorWebFormsComponents](../README.md) (BWFC) library. It combines automated scripts, a Copilot skill, and a decision-making agent into a three-layer pipeline that handles ~85% of migration work mechanically or with AI assistance, leaving you to focus on the architecture decisions that actually need a human.

---

## Who This Is For

You're a .NET developer who owns a Web Forms application and wants to migrate it to Blazor. You know Web Forms well. You may not know Blazor deeply — that's fine. You have GitHub Copilot available and are willing to use it.

---

## Prerequisites

| Requirement | Version | Why |
|---|---|---|
| .NET SDK | 8.0+ | Blazor Server target framework |
| PowerShell | 7.0+ | Migration scripts require PowerShell Core |
| BWFC NuGet package | Latest | `dotnet add package Fritz.BlazorWebFormsComponents` |
| GitHub Copilot | Any tier | Used for Layer 2 structural transforms |

---

## The Three-Layer Pipeline

Migration isn't one step — it's three layers that handle different kinds of work:

| Layer | What | How | Coverage |
|---|---|---|---|
| **Layer 1** — Automated | Tag prefix removal, `runat` removal, expression conversion, file renaming | [`bwfc-migrate.ps1`](../scripts/bwfc-migrate.ps1) | ~40% of work |
| **Layer 2** — Copilot-Assisted | Data binding rewiring, layout conversion, lifecycle method migration | [Copilot migration skill](../.github/skills/webforms-migration/SKILL.md) | ~45% of work |
| **Layer 3** — Architecture Decisions | Identity, EF Core, session state, third-party integrations | [Migration agent](../.github/agents/migration.agent.md) + human judgment | ~15% of work |

**Start here:** [QUICKSTART.md](QUICKSTART.md) — the linear "just tell me what to do" path.

---

## Quick Overview

```
1. Scan     →  ./scripts/bwfc-scan.ps1 -Path ./MyWebFormsApp -OutputFormat Markdown
2. Transform →  ./scripts/bwfc-migrate.ps1 -Path ./MyWebFormsApp -Output ./MyBlazorApp
3. Guide     →  Open in editor with Copilot + BWFC migration skill
4. Verify    →  dotnet build && dotnet run
```

---

## What's In This Toolkit

| Document | What It Covers |
|---|---|
| [**README.md**](README.md) | You are here — overview and entry point |
| [**QUICKSTART.md**](QUICKSTART.md) | Step-by-step: scan → migrate → verify |
| [**CONTROL-COVERAGE.md**](CONTROL-COVERAGE.md) | Full 52-component coverage table with complexity ratings |
| [**METHODOLOGY.md**](METHODOLOGY.md) | Three-layer pipeline deep-dive |
| [**CHECKLIST.md**](CHECKLIST.md) | Per-page migration checklist template |
| [**copilot-instructions-template.md**](copilot-instructions-template.md) | Drop-in `.github/copilot-instructions.md` for your project |

---

## Existing Artifacts (Referenced, Not Duplicated)

The toolkit ties together artifacts that already exist in this repo. You don't need to find them — the docs link to them — but here's the map:

| Artifact | Location | Role |
|---|---|---|
| Scanner script | [`scripts/bwfc-scan.ps1`](../scripts/bwfc-scan.ps1) | Layer 0 — inventories your Web Forms project |
| Migration script | [`scripts/bwfc-migrate.ps1`](../scripts/bwfc-migrate.ps1) | Layer 1 — mechanical transforms |
| Copilot skill | [`.github/skills/webforms-migration/SKILL.md`](../.github/skills/webforms-migration/SKILL.md) | Layer 2 — teaches Copilot the BWFC control mappings |
| Migration agent | [`.github/agents/migration.agent.md`](../.github/agents/migration.agent.md) | Layer 3 — interactive architecture guidance |
| WingtipToys (before) | [`samples/WingtipToys/`](../samples/WingtipToys/) | Reference — original Web Forms application |
| WingtipToys (after) | [`samples/AfterWingtipToys/`](../samples/AfterWingtipToys/) | Reference — migrated Blazor application |

---

## What BWFC Doesn't Cover

Be honest with yourself about scope. BWFC provides 52 drop-in components, but it does **not** cover:

- **DataSource controls** — `SqlDataSource`, `ObjectDataSource`, `EntityDataSource` have no Blazor equivalents. Replace with injected services.
- **Wizard control** — No BWFC equivalent. Implement as a multi-step Blazor component.
- **AJAX Control Toolkit extenders** — Third-party extenders (ModalPopup, AutoComplete, etc.) need Blazor-native replacements.
- **Web Parts** — No equivalent. Redesign as Blazor components.
- **ASP.NET Identity plumbing** — BWFC provides Login/LoginView UI components, but the underlying identity system must be migrated to ASP.NET Core Identity separately.

See [CONTROL-COVERAGE.md](CONTROL-COVERAGE.md) for the full supported/unsupported breakdown.

---

## How Long Will This Take?

Based on the [WingtipToys proof-of-concept](../planning-docs/WINGTIPTOYS-MIGRATION-EXECUTIVE-REPORT.md) (33 pages, 230+ control instances):

| Approach | Estimated Time | Per-Page Average |
|---|---|---|
| Manual rewrite (no BWFC) | 60–80 hours | ~2–2.5 hours |
| **BWFC + three-layer pipeline** | **18–26 hours** | **~35–45 minutes** |
| BWFC + pipeline + AI agents | ~4.5 hours | ~8 minutes |

That's a **55–70% reduction** in migration effort.

---

## Next Steps

1. **[Read the Quickstart](QUICKSTART.md)** — get your first page migrated
2. **[Check control coverage](CONTROL-COVERAGE.md)** — verify your controls are supported
3. **[Copy the Copilot instructions template](copilot-instructions-template.md)** — set up Copilot for your project
