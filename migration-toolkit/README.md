# BWFC Migration Toolkit

**Migrate your ASP.NET Web Forms application to Blazor — systematically, not heroically.**

This toolkit packages everything you need to take a Web Forms app and bring it to Blazor using the [BlazorWebFormsComponents](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents) (BWFC) library. It combines automated scripts, shim infrastructure, Copilot skills, and a decision-making agent into a three-layer pipeline that handles ~90% of migration work mechanically or with AI assistance, leaving you to focus on the architecture decisions that actually need a human.

- **NuGet Package:** <https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents>
- **GitHub Repository:** <https://github.com/FritzAndFriends/BlazorWebFormsComponents>

---

## Who This Is For

You're a .NET developer who owns a Web Forms application and wants to migrate it to Blazor. You know Web Forms well. You may not know Blazor deeply — that's fine. You have GitHub Copilot available and are willing to use it.

---

## Prerequisites

| Requirement | Version | Why |
|---|---|---|
| .NET SDK | 10.0+ | Blazor Server target framework (.NET 10 Global Server Interactive) |
| PowerShell | 7.0+ | Scanner script and lightweight migration alternative |
| BWFC NuGet package | Latest | `dotnet add package Fritz.BlazorWebFormsComponents` |
| GitHub Copilot | Any tier | Used for Layer 2 structural transforms |
| BWFC CLI tool | (included in repo) | Primary Layer 1 transforms — `webforms-to-blazor migrate` |

---

## Key Principle: Shim-First Migration

**The goal is to preserve the original Web Forms API calls, not rewrite them.**

When you migrate with BWFC:

- Pages inherit from `WebFormsPageBase`, which provides `Session`, `Response`, `Request`, `Server`, `Cache`, and `ClientScript` — **all with the SAME API as Web Forms**
- Your code-behind continues to use `Response.Redirect("~/Products")`, `Session["CartId"]`, `Request.QueryString["key"]`, and `IsPostBack` — no changes needed
- `AddBlazorWebFormsComponents()` in `Program.cs` registers all the shim infrastructure automatically

**This means:** If the original Web Forms code says `Session["CartId"]`, the migrated code says `Session["CartId"]`. The shim makes it work. You are not converting to `IMemoryCache`, `NavigationManager`, or `IHttpContextAccessor` — those are native Blazor patterns that you can adopt *later* if desired, but they are not required for migration.

Shims get your app compiling and running **fast**. Refactoring to native Blazor patterns is an optional Layer 3 optimization, not a migration requirement.

---

## How to Use This Toolkit

1. **Copy the `skills/` folder** into your project's `.github/skills/` directory.
2. **Copy the `scripts/` folder** to your project root.
3. Install the BWFC NuGet package: `dotnet add package Fritz.BlazorWebFormsComponents`
4. Follow the [QUICKSTART.md](QUICKSTART.md) guide.

---

## What's Included

### `skills/` — Copilot Skills

Copy these into your project's `.github/skills/` directory so Copilot can use them during migration.

| Skill | Description |
|---|---|
| [`bwfc-migration/SKILL.md`](skills/bwfc-migration/SKILL.md) | **Core markup migration** — control translation, expression conversion, data binding, code-behind lifecycle, and Master Page to Layout conversion. Covers Layer 2 structural transforms (~30% of migration work). |
| [`bwfc-identity-migration/SKILL.md`](skills/bwfc-identity-migration/SKILL.md) | **Identity & auth migration** — OWIN to ASP.NET Core auth middleware, login page migration, BWFC login controls, role-based authorization, and AuthorizeView patterns. |
| [`bwfc-data-migration/SKILL.md`](skills/bwfc-data-migration/SKILL.md) | **Data access & architecture migration** — EF6 to EF Core, DataSource controls to service injection, Session state to scoped services, Global.asax to Program.cs, Web.config to appsettings.json. Covers Layer 3 architecture decisions. |
| [`l3-performance-optimization/SKILL.md`](skills/l3-performance-optimization/SKILL.md) | **Post-migration performance optimization** — async/await modernization, `AsNoTracking()`, `IDbContextFactory`, `[StreamRendering]`, `@key` directives, and .NET 10 patterns. Optional pass after the app builds and runs. |

### `scripts/` — Migration Scripts & CLI Tool

| Tool | Description |
|---|---|
| [`webforms-to-blazor` CLI](../src/BlazorWebFormsComponents.Cli/) | **Primary L1 tool** — 37 compiled C# transforms (16 markup + 11 code-behind + 10 directive/infrastructure) with 373 unit tests. Produces a `migration-report.json` with per-page readiness status. Run via `dotnet run --project src/BlazorWebFormsComponents.Cli -- migrate -i ./MyWebFormsApp -o ./MyBlazorApp`. |
| [`bwfc-scan.ps1`](scripts/bwfc-scan.ps1) | **Scanner** — inventories your Web Forms project, identifies controls, counts pages, and outputs a migration readiness report. Run this first. |
| [`bwfc-migrate.ps1`](scripts/bwfc-migrate.ps1) | **Lightweight alternative** — PowerShell regex transforms for quick starts without building the CLI. Strips `asp:` prefixes, removes `runat="server"`, converts expressions, renames `.aspx`→`.razor`. |

---

## The Three-Layer Pipeline

Migration isn't one step — it's three layers that handle different kinds of work:

| Layer | What | How | Coverage |
|---|---|---|---|
| **Layer 1** — Automated | Tag prefix removal, `runat` removal, expression conversion, file renaming, **shim infrastructure setup** (`_Imports.razor` with `@inherits WebFormsPageBase`, `Program.cs` with `AddBlazorWebFormsComponents()`) | [`webforms-to-blazor` CLI](../src/BlazorWebFormsComponents.Cli/) or [`bwfc-migrate.ps1`](scripts/bwfc-migrate.ps1) | ~60% of work |
| **Layer 2** — Copilot-Assisted | Data binding rewiring, layout conversion, lifecycle method signatures, event handlers — **uses shims to preserve Web Forms patterns** (`Response.Redirect`, `Session`, `Request`, `IsPostBack` all work AS-IS) | [`skills/bwfc-migration/SKILL.md`](skills/bwfc-migration/SKILL.md) | ~30% of work |
| **Layer 3** — Architecture Decisions | Identity, EF Core, third-party integrations | [`skills/bwfc-data-migration/SKILL.md`](skills/bwfc-data-migration/SKILL.md) + human judgment | ~10% of work |
| **L3-opt** — Performance (optional) | Async/await, `AsNoTracking`, `IDbContextFactory`, .NET 10 patterns, **optionally replacing shims with native Blazor patterns** | [`skills/l3-performance-optimization/SKILL.md`](skills/l3-performance-optimization/SKILL.md) | After app is functional |

**Start here:** [QUICKSTART.md](QUICKSTART.md) — the linear "just tell me what to do" path.

---

## Quick Overview

```
1. Scan     →  ./scripts/bwfc-scan.ps1 -Path ./MyWebFormsApp -OutputFormat Markdown
2. Transform →  dotnet run --project src/BlazorWebFormsComponents.Cli -- migrate -i ./MyWebFormsApp -o ./MyBlazorApp
3. Guide     →  Open in editor with Copilot + BWFC migration skill
4. Verify    →  dotnet build && dotnet run
```

> **Alternative:** If you don't have the BWFC source, use `./scripts/bwfc-migrate.ps1 -Path ./MyWebFormsApp -Output ./MyBlazorApp` for Layer 1.

**Shim infrastructure:** `AddBlazorWebFormsComponents()` + `@inherits WebFormsPageBase` enables `Response.Redirect`, `Session["key"]`, `IsPostBack`, `Page.Title`, `Request.QueryString`, `Cache`, `Server.MapPath`, and `ClientScript` to work AS-IS — no manual conversion of these APIs is needed.

---

## Documentation

| Document | What It Covers |
|---|---|
| [**README.md**](README.md) | You are here — overview and entry point |
| [**QUICKSTART.md**](QUICKSTART.md) | Step-by-step: scan → migrate → verify |
| [**CONTROL-COVERAGE.md**](CONTROL-COVERAGE.md) | Full 58-component coverage table with complexity ratings + shim infrastructure inventory |
| [**METHODOLOGY.md**](METHODOLOGY.md) | Three-layer pipeline deep-dive |
| [**CHECKLIST.md**](CHECKLIST.md) | Per-page migration checklist template |
| [**copilot-instructions-template.md**](copilot-instructions-template.md) | Drop-in `.github/copilot-instructions.md` for your project |

---

## What BWFC Doesn't Cover

Be honest with yourself about scope. BWFC provides 58 drop-in components, but it does **not** cover:

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
| **BWFC + three-layer pipeline** | **16–24 hours** | **~30–40 minutes** |
| BWFC + pipeline + AI agents | ~4 hours | ~7 minutes |

That's a **55–70% reduction** in migration effort.

---

## Next Steps

1. **[Read the Quickstart](QUICKSTART.md)** — get your first page migrated
2. **[Check control coverage](CONTROL-COVERAGE.md)** — verify your controls are supported
3. **[Copy the Copilot instructions template](copilot-instructions-template.md)** — set up Copilot for your project
