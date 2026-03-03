# Milestone 22 — Copilot-Led Migration Showcase

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-03-02
**Requested by:** Jeffrey T. Fritz
**Status:** PROPOSED

---

## Executive Summary

M22 shifts BlazorWebFormsComponents from a *component library* to a **migration platform**. The goal is to demonstrate that a real ASP.NET Web Forms application can be migrated to Blazor using BlazorWebFormsComponents with GitHub Copilot guiding the process — producing a working application with minimal manual changes and identical visual/behavioral output.

This milestone is strategic, not just technical. It proves the library's value proposition: **"strip `asp:` and `runat="server"`, keep everything else, and it just works."**

---

## 1. Current State Assessment

### 1.1 Component Inventory

| Category | Count | Status |
|----------|-------|--------|
| Editor Controls | 25 | Complete (2 deferred: Xml, Substitution) |
| Data Controls | 9 | Complete |
| Validation Controls | 7 | Complete |
| Navigation Controls | 3 | Complete |
| Login Controls | 7 | Complete |
| AJAX Controls | 6 | Complete (3 no-op stubs) |
| **Total** | **57** | **51 functional, 6 stubs/deferred** |

### 1.2 Migration Readiness Rating

**Tier 1 — Demo-Ready (high confidence, minimal gaps):** ~35 controls
These controls have same name, same primary attributes, and produce structurally equivalent HTML. A Copilot-guided migration would succeed with these today.

Includes: Button, Label, TextBox, CheckBox, RadioButton, DropDownList, ListBox, HyperLink, Image, ImageButton, LinkButton, Literal, Localize, HiddenField, Panel, PlaceHolder, Table (+ TableRow/TableCell/TableHeaderCell), BulletedList, CheckBoxList, RadioButtonList, AdRotator, FileUpload, MultiView/View, Repeater, DataList, GridView, DataPager, FormView, DetailsView, all 7 Validation Controls, SiteMapPath.

**Tier 2 — Functional but with Known Gaps:** ~10 controls
These work for common scenarios but have divergences or missing features that a realistic migration might hit.

- **ListView**: Missing 16 CRUD events (#356), EditItemTemplate rendering bug (#406)
- **Calendar**: D-13 (day padding) and D-14 (style pass-through) divergences
- **Menu**: Complex dynamic styling, JS-heavy interactions
- **TreeView**: JS-heavy expand/collapse, complex node rendering
- **Chart**: Fundamentally different implementation (Chart.js vs System.Web.UI.DataVisualization)
- **Login Controls (7)**: Require auth provider wiring — functional but not drop-in
- **DataGrid**: Legacy control, functional but rarely used in new migrations

**Tier 3 — Stubs/Deferred:** 6 controls
- ScriptManager, ScriptManagerProxy (no-op stubs — correct behavior for Blazor)
- Substitution (no-op stub)
- Timer, UpdatePanel, UpdateProgress (functional AJAX replacements)
- Xml, Substitution (deferred indefinitely)

### 1.3 Existing Assets

- **BeforeWebForms sample**: 48 control samples across 47 directories, 62 .aspx pages total. Uses .NET Framework, Bootstrap 3, Site.Master layout. Contains GridView, ListView, FormView with real data scenarios.
- **AfterBlazorServerSide sample**: 162+ .razor pages, ComponentCatalog with 47 entries across 11 categories. Comprehensive but organized as individual demos, not a cohesive migrated app.
- **Migration docs**: 7 strategy documents (readme, Strategies, MasterPages, User-Controls, Custom-Controls, NET-Standard, DeferredControls). Outdated in places (references .NET Core 3.1, missing .NET 10 guidance).
- **Copilot instructions**: `.github/copilot-instructions.md` exists with comprehensive project context for development — but not tuned for *migration guidance*.
- **47 planning docs**: Component-level analysis documents covering HTML fidelity, divergences, and audit reports.

---

## 2. The Demo Vision

### 2.1 What We're Demonstrating

A developer (or audience watching Jeff demo) sees:

1. **A running Web Forms application** — the BeforeWebForms sample or a purpose-built "Contoso Widgets" app
2. **GitHub Copilot receives the migration task** — using custom instructions tuned for BWFC migration
3. **Copilot performs the migration step-by-step**:
   - Creates Blazor Server project, adds BWFC NuGet package
   - Migrates Master Page → Blazor Layout
   - Migrates each .aspx page → .razor page (strip `asp:`, strip `runat="server"`, adjust directives)
   - Migrates code-behind → partial class or `@code` block
   - Wires up data access and services
4. **The Blazor app runs** — same pages, same data, same visual layout
5. **Side-by-side comparison** — before/after showing minimal markup changes
6. **"It just works" moment** — CSS, layout, and behavior preserved

### 2.2 Target Application Complexity

**Medium complexity** — representative of a real internal business app:

- 5-8 pages (not 48 — a focused subset that tells the story)
- Master page with navigation, header, footer
- Data-bound controls: GridView with paging/sorting, Repeater, FormView
- Form controls: TextBox, DropDownList, CheckBox, Button with validation
- Navigation: Menu or SiteMapPath
- Authentication: LoginView/LoginStatus (basic)
- Styling: CssClass-based (not Skins/Themes — that's #369 and too complex for the demo)

### 2.3 Controls That MUST Work for the Demo

| Control | Demo Usage | Current State |
|---------|-----------|---------------|
| Button | Form submissions | ✅ Ready |
| TextBox | Form inputs | ✅ Ready |
| Label | Display text | ✅ Ready |
| DropDownList | Filter/select | ✅ Ready |
| CheckBox | Form toggles | ✅ Ready |
| GridView | Data table with paging | ✅ Ready |
| Repeater | Simple list rendering | ✅ Ready |
| FormView | Single-record edit | ✅ Ready |
| RequiredFieldValidator | Form validation | ✅ Ready |
| CompareValidator | Password confirmation | ✅ Ready |
| ValidationSummary | Error display | ✅ Ready |
| Panel | Container/grouping | ✅ Ready |
| HyperLink | Navigation links | ✅ Ready |
| Menu or SiteMapPath | Page navigation | ✅ Ready (SiteMapPath simpler) |
| LoginView/LoginStatus | Auth display | ✅ Ready (needs auth provider) |
| ScriptManager | No-op stub | ✅ Ready (drops silently) |

**All core demo controls are Tier 1 — no blocking component work needed.**

---

## 3. M22 Work Items

### Wave 1: Demo Infrastructure (Priority: CRITICAL)

#### WI-1: Reference Web Forms Application ("Contoso Widgets Manager")
**Size:** Large | **Agent:** Jubilee (samples), Forge (review)

Build a purpose-built Web Forms application (or curate the existing BeforeWebForms sample) that represents a realistic small business app:

- **Home page**: Dashboard with summary labels, links
- **Products page**: GridView with paging, sorting, edit/delete buttons (BoundField + TemplateField)
- **Product Detail page**: FormView with ItemTemplate/EditItemTemplate
- **Order Entry page**: Form with TextBox, DropDownList, CheckBox, validation controls, Button submit
- **Reports page**: Repeater showing formatted data
- **Site.Master**: Header, navigation Menu/SiteMapPath, footer, ContentPlaceHolder
- **Web.sitemap**: Navigation structure
- **Login page**: Login control (optional — LoginView on Master for auth-aware display)

This app must be simple enough to migrate in a live demo (~15-30 minutes) but complex enough to be credible as a "real" application.

**Decision: Use BeforeWebForms as the base.** It already has 48 control samples and a Master Page. Curate a subset of 6-8 pages into a "Contoso Widgets" narrative. This avoids building from scratch while keeping the sample repository useful.

#### WI-2: Copilot Migration Instructions File
**Size:** Medium | **Agent:** Beast (docs), Forge (review)

Create `.github/copilot-migration-instructions.md` — a purpose-built instruction file that Copilot can use as context when performing a Web Forms → Blazor migration. This is NOT the existing `copilot-instructions.md` (which is for developing the library). This is for **consuming** the library during migration.

Contents:
- Step-by-step migration recipe (project setup → NuGet → _Imports → Master→Layout → pages → code-behind → validation → data binding)
- Control-by-control translation table (Web Forms → BWFC equivalent)
- Common patterns: `asp:` removal, `runat="server"` removal, `<%# %>` → `@`, `CodeBehind` → partial class
- Data binding translation: `DataSource` + `DataBind()` → `Items` property
- Event translation: `OnClick` server event → `EventCallback`
- Template translation: `<ItemTemplate>` → `<ItemTemplate>` (same!) but with `Context="Item"` for Web Forms compatibility
- Known gotchas: no ViewState, no PostBack, no DataSource controls, ID renders differently
- Validation: same controls, same attributes, just remove `asp:` and `runat="server"`

#### WI-3: Step-by-Step Migration Walkthrough Document
**Size:** Large | **Agent:** Beast (docs), Forge (review)

Create `docs/Migration/CopilotGuidedMigration.md` — the canonical migration guide, organized as a numbered recipe that both humans and Copilot can follow:

1. Create Blazor project structure
2. Add BWFC NuGet package
3. Configure _Imports.razor
4. Register BlazorWebFormsComponents services
5. Migrate Master Page → Blazor Layout
6. Migrate Web.sitemap → SiteMapPath data
7. Migrate each page (detailed per-control-type instructions)
8. Migrate code-behind files
9. Wire up data access (replace DataSource controls with service injection)
10. Test and verify HTML output

Each step includes before/after code comparison and explains what Copilot should do.

### Wave 2: Demo Polish & Content (Priority: HIGH)

#### WI-4: "After" Blazor Migration Application
**Size:** Large | **Agent:** Jubilee (samples), Cyclops (component fixes if needed), Forge (review)

Create the migrated version of WI-1 as a new sample or curated subset within AfterBlazorServerSide. This is the "after" in the before/after comparison.

- Same pages, same data, same visual layout
- Uses BWFC components exclusively (no raw HTML replacements)
- Demonstrates the minimal-change migration story
- Includes `Context="Item"` pattern for all data-bound templates
- Uses `services.AddBlazorWebFormsComponents()` for JS setup

#### WI-5: Before/After Diff Document
**Size:** Small | **Agent:** Beast (docs)

Create `docs/Migration/BeforeAfterComparison.md` — a visual document showing side-by-side diffs of key pages:
- Master Page → Layout
- GridView page (ASPX → Razor)
- Form page with validation
- Code-behind (CS → partial class)

This document doubles as a talk slide deck supplement and Copilot context.

#### WI-6: Update Migration Docs to .NET 10
**Size:** Medium | **Agent:** Beast (docs)

The existing migration docs reference .NET Core 3.1. Update:
- `docs/Migration/readme.md` — update framework references, add BWFC service registration step
- `docs/Migration/Strategies.md` — update .NET Standard → .NET 10 guidance
- `docs/Migration/MasterPages.md` — update for .NET 10 Blazor layout patterns
- Add `mkdocs.yml` entry for CopilotGuidedMigration.md

#### WI-7: Migration Readiness Checklist
**Size:** Small | **Agent:** Beast (docs), Forge (review)

Create `docs/Migration/MigrationReadinessChecklist.md` — a developer-facing checklist:
- [ ] All business logic in separate class libraries?
- [ ] DataSource controls identified for replacement?
- [ ] Custom controls inventoried?
- [ ] MasterPage structure documented?
- [ ] Third-party controls identified?
- [ ] Web.config dependencies cataloged?

### Wave 3: Component Fixes for Demo (Priority: MEDIUM)

#### WI-8: Fix ListView EditItemTemplate Rendering (#406)
**Size:** Medium | **Agent:** Cyclops (fix), Rogue (test), Forge (review)

**Required for demo?** Only if the demo includes ListView CRUD. For the core demo (GridView-focused), this is optional. But it's a real bug that blocks realistic ListView usage.

The bug: `EditIndex` changes but `ListView.razor` line 59 doesn't pick up the change for template selection during re-render.

#### WI-9: ListView Core CRUD Events (Subset of #356)
**Size:** Medium | **Agent:** Cyclops (implement), Rogue (test), Forge (review)

**Required for demo?** Optional. If the demo includes a ListView CRUD scenario, we need at minimum: ItemEditing, ItemUpdating, ItemDeleting, ItemCanceling (4 of the 16 missing). Full #356 is too large for M22 — pick the 4 essential CRUD events.

#### WI-10: Update Copilot Instructions for Library Development
**Size:** Small | **Agent:** Beast (docs)

Update `.github/copilot-instructions.md` to reference the migration guide and migration instructions file. Add a section: "If a developer asks about migrating a Web Forms app, point them to `docs/Migration/CopilotGuidedMigration.md`."

### Wave 4: Demo Execution (Priority: HIGH, depends on Waves 1-2)

#### WI-11: Demo Script / Walkthrough
**Size:** Medium | **Agent:** Forge (author), Beast (polish)

Create `planning-docs/M22-DEMO-SCRIPT.md` — a timed, annotated script for Jeff's demo:

- **0:00-2:00** — Show the running Web Forms app, highlight controls
- **2:00-5:00** — Open Copilot, paste migration task, show instructions loading
- **5:00-15:00** — Copilot migrates pages one by one (scripted order for best narrative flow)
- **15:00-18:00** — Build and run the Blazor app
- **18:00-22:00** — Side-by-side comparison, highlight minimal changes
- **22:00-25:00** — Show the HTML output matches, CSS works, JavaScript works
- **25:00-30:00** — Q&A setup, mention component inventory, themes roadmap

#### WI-12: Integration Test for Migration Scenario
**Size:** Medium | **Agent:** Rogue (test), Colossus (Playwright)

Create a Playwright integration test that navigates the "after" migrated app and validates:
- All pages load without errors
- GridView renders with data
- Form submission works
- Validation fires correctly
- Navigation works
- Layout matches expected structure

This ensures the demo won't break from future changes.

---

## 4. Scope Decisions

### IN Scope for M22

| Item | Rationale |
|------|-----------|
| Reference Web Forms app (curated subset) | Core demo asset |
| Copilot migration instructions | Core demo enabler |
| Step-by-step migration guide | Core documentation |
| "After" migrated Blazor app | Core demo asset |
| Before/after comparison doc | Demo support material |
| Migration doc updates to .NET 10 | Outdated docs undermine credibility |
| Demo script | Jeff needs a rehearsable walkthrough |
| Integration test for migrated app | Prevents demo-breaking regressions |
| ListView #406 fix | Real bug, visible during demo if ListView used |
| Copilot instructions update | Ensures Copilot gives good migration advice |

### OUT of Scope for M22 (Deferred)

| Item | Why Deferred | Issue |
|------|-------------|-------|
| Full Skins & Themes (#369) | Too complex for demo scope. CssClass-based styling is sufficient for the migration story. Themes are a separate feature milestone. | #369 |
| Full ListView CRUD events (#356) | 16 events is too large. Take 4 essential CRUD events at most. | #356 |
| AJAX Control Toolkit extenders (#297) | Community request, not part of core Web Forms controls. Interesting but not needed for migration demo. | #297 |
| Xml control | XSLT is dead. No realistic migration scenario uses it. | Deferred |
| DataSource controls | Explicitly out of scope per project design. Migration guide covers service injection replacement. | — |
| Wizard control | Not commonly used in business apps. | — |
| Custom control migration tooling | The Roslyn analyzer (BWFC001) exists. Full custom control migration is complex. | — |
| bwfc-migrate CLI tool | M12-M14 planned a migration CLI. Not needed for Copilot-led demo — Copilot IS the migration tool. | — |

### Conditional (Include if Time Allows)

| Item | Condition |
|------|-----------|
| ListView subset CRUD events (WI-9) | Only if demo includes ListView editing |
| Video recording of demo | Only if Jeff wants a pre-recorded demo |
| Copilot Chat extension/agent | If GitHub ships Copilot Extensions for VS before M22 |

---

## 5. Relationship to Open Issues

### #406 — ListView EditItemTemplate Bug
**Verdict: IN M22 (WI-8).** This is a real bug that would embarrass us if someone tries a ListView migration. Fix it regardless of whether the demo uses ListView.

### #369 — Full Skins & Themes
**Verdict: OUT of M22.** The demo should use CssClass-based styling, which is the most common pattern in real Web Forms apps. Full theming support is a separate feature milestone. The migration guide should document: "If your Web Forms app uses Themes, see [future guide] for migration options."

### #356 — ListView CRUD Events
**Verdict: PARTIALLY IN M22 (WI-9, optional).** Take 4 essential CRUD events (ItemEditing, ItemUpdating, ItemDeleting, ItemCanceling) if the demo needs ListView editing. Defer the remaining 12 events.

### #297 — AJAX Control Toolkit Extenders
**Verdict: OUT of M22.** This is about the AJAX Control Toolkit (a separate open-source library), not core ASP.NET Web Forms controls. The migration guide should mention: "AJAX Control Toolkit controls are not supported. Consider Blazor component alternatives."

---

## 6. Success Criteria

M22 is successful when:

1. **Jeff can perform a live migration demo in under 30 minutes** — from a running Web Forms app to a running Blazor app using Copilot + BWFC
2. **The "after" app has the same visual appearance** — same HTML structure, same CSS classes, same layout
3. **The migration required fewer than 20 types of changes** — strip `asp:`, strip `runat`, update directives, wire services — that's the core list
4. **Copilot can perform the migration with the provided instructions** — the `.github/copilot-migration-instructions.md` gives Copilot enough context to do the migration correctly
5. **A developer watching the demo could replicate it** — the migration guide is complete enough to follow independently
6. **No component bugs surface during the demo** — all demo-path controls work correctly

---

## 7. Dependencies and Risks

### Dependencies
- M21 must be complete (current milestone)
- Release infrastructure (M21/PR #408) must be merged — need stable NuGet package for demo
- .NET 10 SDK must be stable (currently in preview — demo should target the release version)

### Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Copilot doesn't follow migration instructions well | Demo fails | Rehearse extensively; pre-stage some changes; have "checkpoint" branches |
| BeforeWebForms sample too complex for 30-minute demo | Demo runs long | Curate a 6-page subset; rehearse timing |
| .NET 10 breaking changes affect BWFC | Components break | Keep testing on nightly builds; have fallback to .NET 9 |
| HTML divergences visible in demo | Credibility hit | Run HTML comparison on demo pages pre-demo; fix critical divergences |
| Auth provider wiring too complex | Login controls fail | Use StaticAuthStateProvider (already exists in sample); skip real auth |

---

## 8. Suggested Timeline

| Week | Focus | Work Items |
|------|-------|------------|
| Week 1 | Foundation | WI-1 (reference app), WI-2 (migration instructions) |
| Week 2 | Documentation | WI-3 (walkthrough), WI-5 (before/after), WI-6 (doc updates) |
| Week 3 | Implementation | WI-4 (after app), WI-8 (ListView fix), WI-10 (Copilot update) |
| Week 4 | Testing & Polish | WI-12 (integration test), WI-7 (readiness checklist), WI-11 (demo script) |
| Week 5 | Rehearsal | Dry-run demos, fix issues, polish script |

---

## 9. Agent Assignments

| Agent | Work Items | Role |
|-------|-----------|------|
| **Forge** | WI-1 (review), WI-2 (review), WI-3 (review), WI-7 (review), WI-11 (author) | Architecture, review, demo script |
| **Cyclops** | WI-8, WI-9 | Component fixes |
| **Jubilee** | WI-1, WI-4 | Sample applications |
| **Beast** | WI-2, WI-3, WI-5, WI-6, WI-7, WI-10 | Documentation |
| **Rogue** | WI-8 (test), WI-9 (test), WI-12 | Testing |
| **Colossus** | WI-12 | Playwright integration tests |

---

## 10. Control-by-Control Migration Translation Table

This table is the heart of the migration story — showing what changes (and what doesn't) for each control:

| Web Forms Control | BWFC Component | Changes Required | Migration Difficulty |
|-------------------|---------------|-----------------|---------------------|
| `<asp:Button>` | `<Button>` | Remove `asp:`, `runat`, `OnClick` → `EventCallback` | Easy |
| `<asp:TextBox>` | `<TextBox>` | Remove `asp:`, `runat`, use `@bind-Text` | Easy |
| `<asp:Label>` | `<Label>` | Remove `asp:`, `runat` | Trivial |
| `<asp:DropDownList>` | `<DropDownList>` | Remove `asp:`, `runat`, Items same | Easy |
| `<asp:CheckBox>` | `<CheckBox>` | Remove `asp:`, `runat` | Easy |
| `<asp:RadioButton>` | `<RadioButton>` | Remove `asp:`, `runat` | Easy |
| `<asp:HyperLink>` | `<HyperLink>` | Remove `asp:`, `runat` | Trivial |
| `<asp:Image>` | `<Image>` | Remove `asp:`, `runat` | Trivial |
| `<asp:Panel>` | `<Panel>` | Remove `asp:`, `runat` | Trivial |
| `<asp:GridView>` | `<GridView>` | Remove `asp:`, `runat`, `DataSource` → `Items`, add `ItemType` | Medium |
| `<asp:Repeater>` | `<Repeater>` | Remove `asp:`, `runat`, `DataSource` → `Items`, add `Context="Item"` | Medium |
| `<asp:FormView>` | `<FormView>` | Remove `asp:`, `runat`, `DataSource` → `DataItem`, add `Context="Item"` | Medium |
| `<asp:ListView>` | `<ListView>` | Remove `asp:`, `runat`, `DataSource` → `Items` | Medium |
| `<asp:RequiredFieldValidator>` | `<RequiredFieldValidator>` | Remove `asp:`, `runat` — everything else same | Easy |
| `<asp:Menu>` | `<Menu>` | Remove `asp:`, `runat`, MenuItems same structure | Easy |
| `<asp:SiteMapPath>` | `<SiteMapPath>` | Remove `asp:`, `runat`, provide SiteMapNode data | Medium |
| `<asp:ScriptManager>` | `<ScriptManager>` | Remove `asp:`, `runat` — renders nothing (correct) | Trivial |
| `<asp:Login>` | `<Login>` | Remove `asp:`, `runat`, wire auth provider | Hard |
| `<asp:Calendar>` | `<Calendar>` | Remove `asp:`, `runat` | Easy |
| `<asp:FileUpload>` | `<FileUpload>` | Remove `asp:`, `runat`, uses InputFile internally | Easy |

---

## Appendix A: Component Inventory Summary

- **57 total controls** implemented (including 6 AJAX controls from M17)
- **51 functional controls** (produce meaningful output)
- **6 stubs or deferred** (3 no-op AJAX stubs correct for Blazor, 2 deferred indefinitely, 1 functional stub)
- **~35 "Tier 1" demo-ready** controls with high migration fidelity
- **~10 "Tier 2"** controls with known gaps (mostly data control events and JS-heavy navigation)
- **1,367+ unit tests** across the library
- **47 planning docs** with component-level analysis
- **48 BeforeWebForms sample pages** as migration source material
- **162+ AfterBlazor sample pages** demonstrating all controls

## Appendix B: Key File Paths

| Asset | Path |
|-------|------|
| Component library | `src/BlazorWebFormsComponents/` |
| Unit tests | `src/BlazorWebFormsComponents.Test/` |
| BeforeWebForms sample | `samples/BeforeWebForms/` |
| AfterBlazor sample | `samples/AfterBlazorServerSide/` |
| Integration tests | `samples/AfterBlazorServerSide.Tests/` |
| Migration docs | `docs/Migration/` |
| Copilot instructions | `.github/copilot-instructions.md` |
| Component catalog | `samples/AfterBlazorServerSide/ComponentCatalog.cs` |
| Planning docs | `planning-docs/` |
| Divergence registry | `planning-docs/DIVERGENCE-REGISTRY.md` |
| Theming infrastructure | `src/BlazorWebFormsComponents/Theming/` |
