# Decision: Deprecation Guidance Documentation (#438)

**Date:** 2026-03-17 (Session by Beast)  
**Issue:** #438 — Add deprecation guidance docs for runat=server, ViewState, UpdatePanel  
**Decision Owner:** Beast (Technical Writer)  
**Status:** SHIPPED

## What We Decided

Create a comprehensive `docs/Migration/DeprecationGuidance.md` page documenting Web Forms patterns that **do not have direct Blazor equivalents** and explaining the Blazor alternative for each.

## Why

Web Forms developers migrating to Blazor encounter patterns they can't directly replicate:
- `runat="server"` — Just a scope marker; Blazor components are always server-side
- `ViewState` — BWFC provides it for compatibility, but developers should use component fields
- `UpdatePanel` — Blazor's component model handles incremental rendering natively
- `ScriptManager` — Migration compatibility stub; use `IJSRuntime` instead
- Page lifecycle (`Page_Load`, `IsPostBack`) — Blazor uses different lifecycle hooks
- Server control property manipulation — Use reactive binding instead

Without guidance, developers might:
- Attempt to recreate obsolete patterns (wasting time)
- Misunderstand why familiar APIs are gone (frustration)
- Miss the Blazor-native alternatives (produce suboptimal code)

## What We Built

**File:** `docs/Migration/DeprecationGuidance.md` (32 KB, ~600 lines)

**Content Structure:**
- Each deprecated pattern has a section: "What It Was" → "Why It's Deprecated" → "What To Do Instead"
- Before/after code examples using markdown tabs for easy comparison
- Pattern-specific migration checklists (e.g., lifecycle mapping table)
- Explanation of why the deprecation makes sense in Blazor architecture

**Patterns Covered:**
1. `runat="server"` — Scope marker; just remove it
2. `ViewState` — Use component fields and scoped services
3. `UpdatePanel` → Blazor incremental rendering (no trigger coordination needed)
4. `Page_Load` / `IsPostBack` → `OnInitializedAsync` / event handlers
5. `ScriptManager` → `IJSRuntime` + `HttpClient` + dependency injection
6. Server control properties → Declarative data binding
7. Application/Session state → Singleton/scoped services
8. Data binding events → Component templates with `@context`

**Navigation Update:**
- Added "Deprecation Guidance" to `mkdocs.yml` Migration section
- Placed after "Automated Migration Guide" (early in the migration journey)

## Tone & Audience

**Audience:** Experienced Web Forms developers learning Blazor

**Tone:**
- ✅ Empathetic — Acknowledges these are familiar patterns being left behind
- ✅ Educational — Explains *why* Blazor works differently
- ✅ Practical — Shows clear, working code examples
- ✅ Non-judgmental — No "ViewState was bad"; just "here's the Blazor way"

**Example language:**
> "In Web Forms, you could manipulate control properties on the server in code-behind... This approach worked because Web Forms re-rendered the entire page on each postback. **Blazor uses reactive data binding.** Control properties are derived from component state, not set imperatively."

## Design Decisions

**Placement in Navigation:**
- *After* Automated Migration Guide (not before)
- *Before* Migration Strategies (guides thinking about approach)
- Rationale: Developers run L1 automation first, then encounter these patterns — this doc addresses them early

**Format: Tabbed Before/After Code:**
- **Why tabbed?** MkDocs markdown supports tabbed syntax; easy to read side-by-side
- **Why always paired?** No Web Forms example without Blazor equivalent
- **Why both `razor` and `csharp` blocks?** Shows markup + code-behind for complete picture

**Lifecycle Mapping Table:**
- Explicit table mapping Web Forms lifecycle to Blazor equivalents
- Helps developers quickly find the right hook (`Page_Load` → `OnInitializedAsync`)

## Success Criteria Met

✅ Covers all 5 core patterns from issue #438:
- runat="server"
- ViewState
- UpdatePanel
- Page_Load / IsPostBack
- ScriptManager

✅ Includes one additional critical pattern:
- Server-side control property manipulation → Reactive binding

✅ Covers derived patterns:
- Application/Session state → Services
- Data binding events → Component templates
- All patterns have before/after code examples

✅ Accessible to Web Forms developers:
- Explains each pattern in Web Forms terms first
- Shows clear "Blazor way" alternative
- Empathetic, non-judgmental tone

## Future Maintenance

**When to update this doc:**
- New BWFC components or patterns added (link to new docs)
- Migration toolkit adds new L1 patterns that lack Blazor equivalents
- Team discovers common migration anti-patterns

**Cross-references to maintain:**
- Migration Strategy Guide (link there from Overview section)
- Automated Migration Guide (link there from Overview section)
- Page Service docs (linked in IsPostBack → lifecycle section)

## Files Modified

1. **Created:** `docs/Migration/DeprecationGuidance.md`
   - 32 KB, ~600 lines
   - 8 major sections + summary checklist
   - Markdown with tabs, admonitions, code blocks

2. **Updated:** `mkdocs.yml`
   - Added "Deprecation Guidance: Migration/DeprecationGuidance.md" entry
   - Positioned after "Automated Migration Guide"

## Related Issues & PRs

- **Linked Issue:** #438
- **Related Skill:** documentation (component-documentation practices applied)
- **Related Docs:** Strategies.md, AutomatedMigration.md, MasterPages.md, User-Controls.md
