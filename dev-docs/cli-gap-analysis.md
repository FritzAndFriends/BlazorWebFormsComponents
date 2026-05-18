# CLI Tool Gap Analysis: `webforms-to-blazor`

> **Author:** Forge (Lead / Web Forms Reviewer)  
> **Date:** 2025-07-27  
> **Scope:** Categorized gap analysis + decision questions for Jeff  
> **Baseline:** 27 transforms (5 directive, 11 markup, 11 code-behind), ~105 tests

---

## Part 1: Gap Analysis

### Category: Missing Transforms

| ID | Gap | Current State | What's Missing | Impact | Complexity |
|----|-----|--------------|----------------|--------|------------|
| **G1** | **MasterPageTransform** not implemented | Architecture doc lists `Markup/MasterPageTransform.cs` for `@inherits LayoutComponentBase`, ContentPlaceHolder→`@Body`, CSS/JS extraction. **No file exists.** The 5 directive transforms handle `<%@ Master %>` removal but don't convert master page *body* structure. | Master page layout conversion (ContentPlaceHolder→`@Body`, `<head>` section extraction, CSS/JS link migration to `App.razor`). Without this, `.master` files produce broken layouts. | **High** | Medium |
| **G2** | **LoginViewTransform** not implemented | Architecture doc lists `Markup/LoginViewTransform.cs` to strip attributes and flag RoleGroups. **No file exists.** | `<asp:LoginView>` with `AnonymousTemplate`, `LoggedInTemplate`, and `RoleGroups` templates are not converted. Migrated apps with auth-gated UI sections will have broken markup. | **Medium** | Small |
| **G3** | **SelectMethodTransform** not implemented | Architecture doc says "preserve attribute + add TODO for delegate conversion." **No file exists.** | `SelectMethod="GetItems"` attributes are left as-is with no TODO guidance. Developer gets no signal that this pattern needs rework. | **Medium** | Small |
| **G4** | **GetRouteUrlTransform** not implemented | Architecture doc says `Page.GetRouteUrl → GetRouteUrlHelper.GetRouteUrl`. **No file exists.** | `Page.GetRouteUrl("RouteName", params)` calls pass through untouched, producing compile errors. | **Medium** | Small |

### Category: Incomplete Transforms

| ID | Gap | Current State | What's Missing | Impact | Complexity |
|----|-----|--------------|----------------|--------|------------|
| **G5** | **AJAX Toolkit — limited control mapping** | `AjaxToolkitPrefixTransform` maps 16 controls (Accordion, TabContainer, Modal­Popup­Extender, etc.) and replaces unknowns with `@* TODO *@` comments. | The AJAX Control Toolkit has 40+ controls. The 24+ unmapped controls (e.g., `ReorderList`, `Rating`, `AsyncFileUpload`, `ComboBox`, `ListSearchExtender`, `DragPanel`, `Animation`) all become TODO comments with no specific guidance. | **Medium** — depends on which controls the source project actually uses | Small per control |
| **G6** | **Complex IsPostBack guards** | Brace-counting unwrap works for simple `if (!IsPostBack) { ... }`. Complex cases (with `else` branch, single-statement without braces) get a TODO annotation but are not transformed. | `if (!IsPostBack) { /* init */ } else { /* postback */ }` is the *common* pattern in real apps. The "else" branch often contains event-handling logic that should map to Blazor event callbacks. The TODO says "move else body to event handler" but doesn't restructure anything. | **High** — nearly every non-trivial Page_Load has an else branch | Large |
| **G7** | **Session/ViewState — detection only** | `SessionDetectTransform` and `ViewStateDetectTransform` find `Session["key"]` / `ViewState["key"]` usage and inject guidance comments listing options (ProtectedSessionStorage, scoped service, etc.). **They do not modify any code.** | The actual `Session["key"]` and `ViewState["key"]` calls are untouched. Code won't compile until developer manually replaces every reference. For ViewState, the guidance mentions `BaseWebFormsComponent.ViewState` as a compatibility shim but doesn't wire it up. For Session, BWFC ships a `SessionShim` class but the CLI doesn't inject `[Inject] SessionShim Session`. | **High** — every Session/ViewState usage requires manual intervention | Medium |
| **G8** | **DataSourceID → TODO only** | `DataSourceIdTransform` removes `DataSourceID="..."` attributes and replaces 7 data source control declarations (`SqlDataSource`, `ObjectDataSource`, `LinqDataSource`, `EntityDataSource`, `XmlDataSource`, `SiteMapDataSource`, `AccessDataSource`) with TODO comments. | No wiring of replacement data flow. Developer must manually create services, inject them, and bind data. The TODO says "wire data through code-behind service injection and SelectMethod/Items" but doesn't scaffold any of this. | **High** — any data-driven page breaks completely | Large |
| **G9** | **Expression parsing — limited depth** | `ExpressionTransform` handles: comments, `Bind("Prop")`, `Eval("prop")` with format strings, `Item.Prop`, `String.Format` with `Item`, encoded `<%: %>`, unencoded `<%= %>`. | Complex/nested expressions fail: `Eval("Nested.Field")` (dotted paths), `Bind()` with method calls, multi-line expressions, ternary operators inside expressions, concatenated expressions. These pass through as broken Razor syntax. | **Medium** — simple pages work fine; complex GridView/FormView templates break | Medium |
| **G10** | **UrlReferenceTransform — limited attributes** | Converts `~/` to `/` for only 3 attributes: `href`, `NavigateUrl`, `ImageUrl`. | Other URL attributes (`src`, `SrcSet`, `BackImageUrl`, `PostBackUrl`, `DataNavigateUrlFormatString`) keep the `~/` prefix, which Blazor doesn't resolve. | **Low** — easy to add more attributes, and remaining ones are less common | Small |
| **G11** | **Code-behind conversion — structural limits** | Transforms handle: using strips, base class removal, Response.Redirect, lifecycle methods, event handler signatures, DataBind patterns, URL cleanup. The `TodoHeaderTransform` injects a 13-item migration checklist. | `.ascx.cs` files get the same transforms as `.aspx.cs` but User Control-specific patterns aren't addressed: `FindControl()` calls, `NamingContainer` references, dynamic control creation via `Controls.Add()`, `LoadControl()`. Also: no conversion of `HttpContext.Current.*` (beyond Session), `ConfigurationManager.AppSettings["key"]`, or `Server.MapPath()`. | **Medium** — most code-behind files need manual cleanup regardless | Medium |

### Category: Pipeline / Tooling

| ID | Gap | Current State | What's Missing | Impact | Complexity |
|----|-----|--------------|----------------|--------|------------|
| **G12** | **Prescanner not implemented** | Architecture doc specifies `Analysis/Prescanner.cs` for BWFC001–BWFC014 pattern analysis. **The `Analysis/` directory is empty.** | No pre-scan analysis means: (a) the migration report can't estimate complexity or flag high-risk files before transforms run, (b) Copilot L2 skills don't get structured prescan data to prioritize work, (c) no BWFC diagnostic codes for CI gating. | **High** — this is the intelligence layer that feeds both reporting and Copilot orchestration | Medium |
| **G13** | **Migration report — flat and uncategorized** | `MigrationReport.cs` tracks: FilesProcessed, FilesWritten, TransformsApplied, ScaffoldFilesGenerated, StaticFilesCopied, Errors, Warnings, ManualItems. ManualItems is `List<string>` — unstructured text. | No per-file breakdown. No categorization of manual items by type (Session, ViewState, DataSource, Identity, etc.). No severity/priority. No mapping to Copilot skill names for L2 handoff. No "estimated completeness" percentage. The JSON report is a flat bag — Copilot skills can't parse it to find "all Session items in file X". The `--report-format markdown` option mentioned in the CLI spec is not implemented. | **High** — directly limits Copilot L2 orchestration effectiveness | Medium |
| **G14** | **No `analyze` / pre-migration readiness command** | Architecture doc says "no public `analyze` subcommand — analysis runs automatically as part of `migrate`." But since the Prescanner doesn't exist (G12), there's effectively no analysis at all. | A standalone analysis command would let developers assess migration complexity *before* committing to the migration. CI pipelines could gate on "readiness score." | **Medium** — nice-to-have; not blocking core migration | Small |
| **G15** | **No Copilot SDK integration** | Architecture doc specifies CLI is "pure L1 deterministic engine" with structured TODO comments for Copilot consumption. No AI integration is planned in the tool itself. | The TODO comments emitted by transforms don't use the `bwfc-*` prefix convention consistently. Some use `// TODO:`, some use `/* TODO: */`, some use `@* TODO: *@`. Copilot skills need a reliable, parseable pattern to find and act on flagged items. | **Medium** — solvable with a naming convention, no AI needed | Small |

### Category: Shims

| ID | Gap | Current State | What's Missing | Impact | Complexity |
|----|-----|--------------|----------------|--------|------------|
| **G16** | **Generated shims are minimal; library shims are rich** | The CLI's `ShimGenerator` outputs two files: `WebFormsShims.cs` (just a `using BlazorWebFormsComponents;` statement + ConfigurationManager comment) and `IdentityShims.cs` (TODO comments mapping 4 Membership patterns). Meanwhile, BWFC's built-in shims are comprehensive: `SessionShim`, `CacheShim`, `ResponseShim`, `RequestShim`, `ServerShim`, `GridViewRowShim`, plus `ShimControls` (Panel, PlaceHolder, HtmlGenericControl). | The generated `WebFormsShims.cs` doesn't register or inject any of the library's shim classes. It doesn't generate DI registration code (`builder.Services.AddScoped<SessionShim>()`, etc.). The rich shims exist in the library but the CLI doesn't bridge migrated code to use them. | **High** — the shims are the whole point of "compile-and-run after L1" but they aren't wired up | Medium |
| **G17** | **No `ConfigurationManager` shim class** | `WebFormsShims.cs` has a *comment* about ConfigurationManager mapping to IConfiguration. No actual shim class exists in the BWFC library. | `ConfigurationManager.AppSettings["key"]` and `ConfigurationManager.ConnectionStrings["name"]` are extremely common in Web Forms code-behind. Without a working shim, every reference is a compile error. | **High** — nearly every real-world Web Forms app uses ConfigurationManager | Medium |

### Category: Test Coverage

| ID | Gap | Current State | What's Missing | Impact | Complexity |
|----|-----|--------------|----------------|--------|------------|
| **G18** | **9 transforms lack unit tests** | **Markup:** AjaxToolkitPrefix, AttributeNormalize, DataSourceId, EventWiring, TemplatePlaceholder (5). **Code-behind:** BaseClassStrip, DataBind, TodoHeader, UsingStrip (4). These rely solely on L1 acceptance tests. | Individual edge cases can't be tested in isolation. When L1 tests fail, it's hard to isolate *which* transform broke. The 4 missing markup transforms (G1–G4) have zero tests since they don't exist. | **Medium** — technical debt that slows debugging | Medium |
| **G19** | **Directive transforms under-tested** | Only `PageDirectiveTransform` has 3 unit tests. `MasterDirectiveTransform`, `ControlDirectiveTransform`, `RegisterDirectiveTransform`, `ImportDirectiveTransform` have no dedicated unit tests. | Directive parsing edge cases (multiple directives on one line, complex attribute quoting, multi-line directives) may break silently. | **Low** — directives are relatively simple regex patterns | Small |

---

## Part 2: Decision Questions for Jeff

### Q1: Master Page Conversion Strategy (relates to G1)

The `MasterPageTransform` is the biggest missing piece. Two approaches:

- **Option A: Full structural conversion** — Convert `ContentPlaceHolder` → `@Body`, extract `<head>` content into `HeadContent`, move CSS/JS links to `App.razor`, emit `@inherits LayoutComponentBase`. This is what the architecture doc specifies.
- **Option B: Minimal wrapper** — Convert master pages to layouts with just `@Body` substitution and leave `<head>` content in-place (works for Blazor Server SSR).

**Question:** Do you want full App.razor integration (Option A) or is the simpler layout conversion (Option B) sufficient for L1? If A, should CSS/JS detection go into the master page transform or stay in `ProjectScaffolder`?

### Q2: SessionShim / ViewState Wiring Strategy (relates to G7, G16)

The BWFC library ships rich shims (`SessionShim`, `CacheShim`, etc.) but the CLI doesn't wire migrated code to use them. Three options:

- **Option A: Detect-and-inject** — When the CLI finds `Session["key"]`, automatically add `[Inject] private SessionShim Session { get; set; }` and leave the `Session["key"]` calls in-place (they'll compile against the shim's indexer). Same for `ViewState` → use BWFC's `BaseWebFormsComponent.ViewState`.
- **Option B: Detect-and-comment** — Current approach. Just inject guidance. Developer does the wiring.
- **Option C: Hybrid** — Inject the `[Inject]` property for Session/Cache (since shims have matching APIs) but leave ViewState as guidance-only (since the shim is `[Obsolete]` and not a long-term solution).

**Question:** Should the CLI auto-wire shim injection (Option A/C) or keep it guidance-only (Option B)? If A or C, should the CLI also generate the DI registration calls in `Program.cs` (e.g., `builder.Services.AddScoped<SessionShim>()`)?

### Q3: TODO Comment Convention (relates to G15)

Current transforms emit TODOs inconsistently:
- `// TODO: BWFC — IsPostBack guard...`
- `/* TODO: Verify navigation target */`
- `@* TODO: Convert ajaxToolkit:X *@`
- `// TODO: Review lifecycle conversion`

**Question:** Should we standardize on a `// TODO(bwfc-category):` format as the architecture doc specifies? If so, what are the category slugs? Proposed list:
- `bwfc-session-state`, `bwfc-viewstate`, `bwfc-identity`, `bwfc-datasource`, `bwfc-ispostback`, `bwfc-lifecycle`, `bwfc-ajax-toolkit`, `bwfc-navigation`, `bwfc-route-url`, `bwfc-expression`, `bwfc-master-page`, `bwfc-login-view`, `bwfc-select-method`

This directly impacts Copilot L2 skill matching.

### Q4: ConfigurationManager Shim (relates to G17)

`ConfigurationManager.AppSettings["key"]` is ubiquitous in Web Forms. Options:

- **Option A: Build a `ConfigurationManagerShim`** class in the BWFC library that wraps `IConfiguration` and provides `AppSettings[key]` and `ConnectionStrings[name]` indexers. CLI injects it.
- **Option B: Rewrite in the transform** — `ConfigurationManager.AppSettings["key"]` → `Configuration["key"]` with `[Inject] IConfiguration Configuration`. More idiomatic but higher-risk regex transform.
- **Option C: Leave as guidance** — just add a TODO. Developer migrates config access manually.

**Recommendation:** Option A aligns with the shim philosophy (compile first, optimize later). The CLI already generates a `WebFormsShims.cs` that mentions ConfigurationManager; it just needs a real class behind it.

### Q5: Migration Report Structure for Copilot L2 (relates to G12, G13)

The report is currently `List<string>` for manual items. For Copilot orchestration, it needs structure. Proposed enhancement:

```json
{
  "manualItems": [
    {
      "file": "Default.aspx.cs",
      "line": 42,
      "category": "bwfc-session-state",
      "description": "Session[\"CartId\"] — convert to SessionShim or scoped service",
      "severity": "high"
    }
  ]
}
```

**Question:** Is this the right schema? Should `category` match skill names exactly? Should we add an `estimatedCompleteness` percentage (e.g., "this file is ~70% migrated")?

### Q6: Prescanner — Build or Defer? (relates to G12, G14)

The Prescanner (BWFC001–BWFC014 analysis) was spec'd but never built. It would:
- Count controls, expressions, code-behind patterns per file
- Assign a complexity score
- Flag high-risk files (data sources, identity, AJAX Toolkit)
- Power a future `analyze` subcommand

**Question:** Should the Prescanner be built now (it feeds report quality and Copilot handoff) or deferred until after the 4 missing transforms (G1–G4) are implemented? The Prescanner doesn't block migrations — it blocks *informed* migrations.

### Q7: DataSource Replacement Depth (relates to G8)

`DataSourceIdTransform` removes declarations and attributes but scaffolds nothing. Options:

- **Option A: Service stub generation** — For each `SqlDataSource` found, generate a stub service class (`I{ControlId}DataService`) with the SELECT query as a comment, inject it into the code-behind, and wire `Items="@_data"` on the bound control.
- **Option B: Keep as TODO** — Current approach. Let Copilot L2 or the developer handle it.
- **Option C: Hybrid** — Generate the service interface + inject site, leave implementation as TODO. At least the code compiles with empty data.

**Question:** How deep should L1 go on data source replacement? Option A is significant new work. Option C gives compile-ability without correctness.

### Q8: Missing Transforms Priority (relates to G1–G4)

Four transforms are spec'd but not built. Recommended priority:

1. **MasterPageTransform** (G1) — blocks any app using master pages (virtually all of them)
2. **LoginViewTransform** (G2) — blocks any app with auth-gated UI
3. **GetRouteUrlTransform** (G4) — causes compile errors
4. **SelectMethodTransform** (G3) — existing attributes pass through harmlessly; lowest urgency

**Question:** Agree with this priority? Should any of these be pushed to L2 (Copilot) instead of being deterministic L1 transforms?

### Q9: IsPostBack Else-Branch Strategy (relates to G6)

The `else` branch in `if (!IsPostBack) { } else { }` is the most common complex case. In Web Forms semantics:
- `if` body = first-load initialization
- `else` body = postback handling (button clicks, etc.)

In Blazor:
- `if` body → `OnInitializedAsync()` (already handled)
- `else` body → event handler methods (but which events? depends on context)

**Question:** Should the CLI attempt to extract the `else` body into a separate method (e.g., `HandlePostBack()`) and leave a TODO to wire it to specific events? Or is the current TODO-only approach adequate given that Copilot L2 can reason about the context?

### Q10: Unit Test Debt (relates to G18, G19)

9 transforms lack unit tests. Options:

- **Option A: Write unit tests before adding new features** — Good discipline, prevents regressions
- **Option B: Write unit tests alongside new features** — Faster feature delivery, tests come with the work
- **Option C: Rely on L1 acceptance tests** — Current approach. Unit tests are nice-to-have.

**Recommendation:** Option B. The 4 missing transforms (G1–G4) should ship with unit tests. Backfill existing gaps opportunistically.

---

## Summary Matrix

| Gap | Impact | Complexity | Recommended Action |
|-----|--------|------------|-------------------|
| G1 MasterPageTransform | High | Medium | **Build now** (Q1, Q8) |
| G2 LoginViewTransform | Medium | Small | Build now (Q8) |
| G3 SelectMethodTransform | Medium | Small | Build now (Q8) |
| G4 GetRouteUrlTransform | Medium | Small | Build now (Q8) |
| G5 AJAX Toolkit controls | Medium | Small | Expand incrementally |
| G6 IsPostBack else-branch | High | Large | Decide strategy (Q9) |
| G7 Session/ViewState wiring | High | Medium | Decide strategy (Q2) |
| G8 DataSource scaffolding | High | Large | Decide strategy (Q7) |
| G9 Expression depth | Medium | Medium | Improve incrementally |
| G10 URL attributes | Low | Small | Quick fix |
| G11 Code-behind structural | Medium | Medium | Improve incrementally |
| G12 Prescanner | High | Medium | Decide timing (Q6) |
| G13 Report structure | High | Medium | Build with Prescanner (Q5) |
| G14 Analyze command | Medium | Small | Defer to after Prescanner |
| G15 TODO conventions | Medium | Small | **Standardize now** (Q3) |
| G16 Shim wiring | High | Medium | Decide strategy (Q2) |
| G17 ConfigurationManager | High | Medium | Decide approach (Q4) |
| G18 Unit test gaps | Medium | Medium | Backfill with features (Q10) |
| G19 Directive test gaps | Low | Small | Backfill opportunistically |
