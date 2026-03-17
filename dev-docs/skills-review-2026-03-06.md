# Skills Cross-Reference Review — 2026-03-06

**Reviewer:** Beast (Technical Writer)
**Requested by:** Jeffrey T. Fritz
**Scope:** All 8 migration/ai-team skill files + 4 supporting migration-toolkit docs

---

## Skills Reviewed

| # | Skill File | Status |
|---|-----------|--------|
| 1 | `migration-toolkit/skills/bwfc-migration/SKILL.md` | ✅ Updated |
| 2 | `migration-toolkit/skills/bwfc-data-migration/SKILL.md` | ✅ Accurate — no changes needed |
| 3 | `migration-toolkit/skills/bwfc-identity-migration/SKILL.md` | ✅ Updated |
| 4 | `migration-toolkit/skills/migration-standards/SKILL.md` | ✅ Updated |
| 5 | `.ai-team/skills/migration-standards/SKILL.md` | ✅ Updated (major) |
| 6 | `.ai-team/skills/base-class-upgrade/SKILL.md` | ✅ Accurate — no changes needed |
| 7 | `.ai-team/skills/webforms-html-audit/SKILL.md` | ✅ Accurate — no changes needed |
| 8 | (no 8th skill file found — 7 total) | N/A |

### Supporting Docs Reviewed

| Doc | Status |
|-----|--------|
| `migration-toolkit/QUICKSTART.md` | ✅ Updated |
| `migration-toolkit/METHODOLOGY.md` | ✅ Accurate (already updated) |
| `migration-toolkit/CHECKLIST.md` | ✅ Updated |
| `migration-toolkit/copilot-instructions-template.md` | ✅ Updated |

---

## Issues Found Per Skill

### 1. `.ai-team/skills/migration-standards/SKILL.md` — 7 issues (CRITICAL)

This was the most stale skill. It had not been updated after the WebFormsPageBase implementation.

| Category | Issue | Fix |
|----------|-------|-----|
| **Incorrect** | Base class listed as `ComponentBase` — should be `WebFormsPageBase` for pages | Updated Target Architecture table |
| **Stale** | `IsPostBack` mapping: "First render check via `firstRender` param" | Updated to "works AS-IS via `WebFormsPageBase`" |
| **Stale** | `Page.Title` mapping: "`<PageTitle>` component" | Updated to "`Page.Title = 'X'` works AS-IS via `WebFormsPageBase`" |
| **Incorrect** | Layer 1: `LoginView → AuthorizeView` | Changed to "preserve as BWFC LoginView" — LoginView uses `AuthenticationStateProvider` natively |
| **Missing** | No render mode placement guidance | Added full Render Mode Placement section |
| **Missing** | No Page Base Class section | Added comprehensive Page Base Class section with properties table |
| **Stale** | Anti-pattern: `// RIGHT: ComponentBase` | Updated to show `WebFormsPageBase` for pages, `ComponentBase` for non-pages |

### 2. `migration-toolkit/skills/migration-standards/SKILL.md` — 1 issue

| Category | Issue | Fix |
|----------|-------|-----|
| **Incorrect** | Layer 1: `LoginView → AuthorizeView` | Changed to "preserve as BWFC LoginView" |

### 3. `migration-toolkit/skills/bwfc-identity-migration/SKILL.md` — 2 issues

| Category | Issue | Fix |
|----------|-------|-----|
| **Stale** | LoginView section labeled "option 2: AuthorizeView (recommended long-term)" | Rewritten — BWFC LoginView is recommended; AuthorizeView shown as alternative |
| **Missing** | No mention that BWFC login controls use `AuthenticationStateProvider` natively | Added note explaining LoginView is not a shim, and service registration requirement |

### 4. `migration-toolkit/skills/bwfc-migration/SKILL.md` — 6 missing features

This skill was well-maintained for core migration patterns but was missing several BWFC features.

| Category | Issue | Fix |
|----------|-------|-----|
| **Missing** | `WebFormsPage` (unified NamingContainer + ThemeProvider + head rendering) | Added to new "Structural & Infrastructure Components" section |
| **Missing** | `MasterPage` / `Content` / `ContentPlaceHolder` BWFC components | Added to structural components table + note in Master Page section |
| **Missing** | `DataBinder.Eval` compatibility shim | Added dedicated section with before/after/recommended patterns |
| **Missing** | `NamingContainer` component | Added to structural components table |
| **Missing** | Theming infrastructure (ThemeProvider, ThemeConfiguration, ControlSkin, SkinBuilder) | Added Theming Infrastructure section |
| **Missing** | Custom control base classes (WebControl, CompositeControl, HtmlTextWriter) | Added Custom Control Base Classes section |
| **Missing** | `EmptyLayout` component | Added to structural components table |
| **Missing** | CSS `<link>` from master `<head>` → `App.razor` guidance | Added to Master Page Key Changes |

### 5. `migration-toolkit/skills/bwfc-data-migration/SKILL.md` — 0 issues

Fully accurate. `AddBlazorWebFormsComponents()` is correctly referenced. EF Core version 10.0.3 is correct. All patterns match actual BWFC code.

### 6. `.ai-team/skills/base-class-upgrade/SKILL.md` — 0 issues

Internal development skill. Patterns for BaseStyledComponent upgrade are accurate. `BaseStyledComponent` extends `BaseWebFormsComponent`, `Style` is a protected computed getter, `SetFontsFromAttributes` is a real extension method.

### 7. `.ai-team/skills/webforms-html-audit/SKILL.md` — 0 issues

Testing/validation skill. HTML normalization rules and control classification patterns are accurate.

---

## Supporting Docs Issues

### `migration-toolkit/QUICKSTART.md` — 3 issues

| Category | Issue | Fix |
|----------|-------|-----|
| **Stale** | SDK prereq says ".NET 8+" | Updated to ".NET 10+" |
| **Missing** | Step 4 `_Imports.razor` missing `@inherits WebFormsPageBase` and `@using static` | Added full `_Imports.razor` with base class and render mode using |
| **Missing** | Step 4 missing `<Page />` in layout | Added layout section with `<BlazorWebFormsComponents.Page />` |
| **Stale** | Layer 2 says "remove IsPostBack checks" | Changed to "works AS-IS via WebFormsPageBase" |

### `migration-toolkit/METHODOLOGY.md` — 0 issues

Already updated with WebFormsPageBase references (line 75).

### `migration-toolkit/CHECKLIST.md` — 1 issue

| Category | Issue | Fix |
|----------|-------|-----|
| **Stale** | Layer 2 checklist says "`IsPostBack` checks removed" | Changed to "`if (!IsPostBack)` works AS-IS via WebFormsPageBase (optionally simplify)" |

### `migration-toolkit/copilot-instructions-template.md` — 2 issues

| Category | Issue | Fix |
|----------|-------|-----|
| **Stale** | Code-behind table: `if (!IsPostBack)` → "Remove" | Changed to "Works AS-IS via WebFormsPageBase" |
| **Stale** | Gotcha #2: "No PostBack — There is no IsPostBack" | Rewritten: "IsPostBack works AS-IS" via WebFormsPageBase |

---

## Cross-Reference: Code vs Skills Coverage

### Features Verified Against Source Code

| Source File | Feature | Covered In Skill? |
|------------|---------|-------------------|
| `WebFormsPageBase.cs` | `Title`, `MetaDescription`, `MetaKeywords`, `IsPostBack`, `Page` self-ref | ✅ bwfc-migration, migration-standards (both versions) |
| `ServiceCollectionExtensions.cs` | `AddBlazorWebFormsComponents()` registers `BlazorWebFormsJsInterop` + `IPageService` | ✅ bwfc-migration, bwfc-data-migration |
| `MasterPage.razor.cs` | `ChildContent`, `Head` params, `ContentPlaceHolders` dictionary | ✅ Now added to bwfc-migration |
| `Content.razor.cs` | `ContentPlaceHolderID` param, cascading `MasterPage` | ✅ Now added to bwfc-migration |
| `DataBinder.cs` | `Eval(string, string)`, `GetPropertyValue`, `GetDataItem` (obsolete) | ✅ Now added to bwfc-migration |
| `LoginView.razor.cs` | Injects `AuthenticationStateProvider` — native Blazor auth | ✅ Fixed in bwfc-identity-migration |
| `LoginStatus.razor.cs` | Injects `AuthenticationStateProvider` + `NavigationManager` | ✅ Implicitly covered |
| `GridView.razor.cs` | Generic `GridView<ItemType>`, `Items` (from DataBoundComponent), `AutoGenerateColumns`, `DataKeyNames` | ✅ bwfc-migration |
| `ListView.razor.cs` | Generic `ListView<ItemType>`, `Items`, `ItemTemplate`, `GroupItemCount` | ✅ bwfc-migration |
| `FormView.razor.cs` | Generic `FormView<ItemType>`, `ItemTemplate`, `EditItemTemplate`, `InsertItemTemplate` | ✅ bwfc-migration |
| `Repeater.razor.cs` | Generic `Repeater<ItemType>`, `ItemTemplate`, `HeaderTemplate`, `FooterTemplate` | ✅ bwfc-migration |
| `NamingContainer.razor.cs` | `ChildContent`, `UseCtl00Prefix`, `ClientIDMode` | ✅ Now added to bwfc-migration |
| `WebFormsPage.razor.cs` | `Theme` param, `RenderPageHead` param, subscribes to IPageService events | ✅ Now added to bwfc-migration |
| `EmptyLayout.razor` | `@inherits LayoutComponentBase` + `@Body` | ✅ Now added to bwfc-migration |
| `IPageService.cs` | `Title`, `MetaDescription`, `MetaKeywords`, change events | ✅ Implicitly covered via WebFormsPageBase docs |
| `Page.razor.cs` | Standalone head renderer subscribing to IPageService | ✅ bwfc-migration |

### Enum Coverage Verification

All enums referenced in skills were verified to exist in `src/BlazorWebFormsComponents/Enums/`:
- `TextBoxMode` (contains `Multiline`) ✅
- `FormViewMode` ✅
- `LogoutAction` ✅
- `ClientIDMode` ✅
- `BorderStyle`, `GridLines`, `HorizontalAlign`, `VerticalAlign`, `RepeatDirection`, `RepeatLayout`, `Orientation` ✅

### Features Still Not Covered by Any Skill

These are lower-priority items that exist in the BWFC codebase but aren't documented in any migration skill:

1. **`Localize` component** — Exists as an editor control but not mentioned in migration patterns
2. **`AdRotator` component** — In component coverage table but no migration guidance
3. **`ImageMap` / HotSpot classes** — In component coverage table but no migration guidance
4. **Style infrastructure** (`IStyle`, `IHasLayoutStyle`, `IFontStyle`, `IHasTableItemStyle`) — Internal interfaces; typically not needed by migrators
5. **`DataBinding/` folder** — Data binding infrastructure (internal)
6. **`ColorTranslator`, `FontUnit`, `Unit`, `WebColor`** — Web Forms type compatibility shims (internal)
7. **`ComponentIdGenerator`** — Internal ID generation service
8. **`BlazorWebFormsJsInterop`** — Internal JS interop service (registered via `AddBlazorWebFormsComponents`)

> **Assessment:** Items 1-3 are in the component coverage table already. Items 4-8 are internal implementation details that migrators don't need to know about. No additional skill coverage needed for these.

---

## Summary of Changes Made

| File | Changes |
|------|---------|
| `.ai-team/skills/migration-standards/SKILL.md` | 7 fixes: base class, IsPostBack, Page.Title, LoginView, render mode, Page Base Class section, anti-pattern |
| `migration-toolkit/skills/migration-standards/SKILL.md` | 1 fix: LoginView → preserve (not AuthorizeView) |
| `migration-toolkit/skills/bwfc-identity-migration/SKILL.md` | 2 fixes: LoginView recommendation, service registration note |
| `migration-toolkit/skills/bwfc-migration/SKILL.md` | Added 3 new sections: Structural Components, DataBinder.Eval, Theming, Custom Controls. Added Master Page migration tips. |
| `migration-toolkit/QUICKSTART.md` | 3 fixes: SDK version, WebFormsPageBase in _Imports, Page component in layout, IsPostBack |
| `migration-toolkit/CHECKLIST.md` | 1 fix: IsPostBack language |
| `migration-toolkit/copilot-instructions-template.md` | 2 fixes: IsPostBack code-behind table, PostBack gotcha |

**Total: 7 files modified, 16+ individual fixes applied.**
