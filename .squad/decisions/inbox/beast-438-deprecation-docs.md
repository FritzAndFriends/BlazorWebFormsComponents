# Beast Decision: Deprecation Guidance Documentation Structure

**Date:** 2026-03-XX  
**Requester:** Jeffrey T. Fritz (Issue #438)  
**Owner:** Beast (Technical Writer)  

## Decision

Created comprehensive deprecation guidance documentation (`docs/Migration/DeprecationGuidance.md`) for Web Forms patterns without direct Blazor equivalents.

## Rationale

1. **Developer Pain Point:** Migrating developers encounter Web Forms patterns that don't exist in Blazor (`ViewState`, `UpdatePanel`, `runat="server"`, etc.) and need clear guidance on Blazor-native alternatives.

2. **Placement Strategy:** Positioned after "Automated Migration Guide" in mkdocs.yml navigation. This catches developers early and provides strategic guidance before they hit specific component documentation.

3. **Format Choice:** Used tabbed before/after code examples for each pattern. This matches modern web frameworks' documentation style and aligns with existing BWFC docs patterns (seen in MasterPages.md, UpdatePanel.md).

4. **Comprehensive Coverage:** Covers not just features but also patterns and idioms:
   - Server-side constructs (`runat="server"`, event model)
   - State management (`ViewState`, `Application`, `Session`)
   - Framework features (`UpdatePanel`, `ScriptManager`)
   - Lifecycle patterns (`Page_Load`, `IsPostBack`)
   - UI update coordination (`ItemDataBound` events)

5. **Audience Fit:** Written for experienced Web Forms developers learning Blazor—emphasizes concepts they already know and maps them to Blazor equivalents rather than explaining Blazor from scratch.

## Implementation Details

**File:** `docs/Migration/DeprecationGuidance.md`
- 23.3 KB, ~400 lines
- 10 major sections, each with Web Forms pattern → Blazor solution
- Tabbed code examples (=== "Web Forms" / === "Blazor" format)
- Migration checklist table at end
- Lifecycle mapping table for Page lifecycle events
- Service lifetime reference table for state management

**Navigation:** Updated `mkdocs.yml` Migration section to include:
```yaml
- Deprecation Guidance: Migration/DeprecationGuidance.md
```
Positioned as second item, right after "Automated Migration Guide" and before "Migration Strategies".

**Branch:** `squad/438-deprecation-docs` (pushed to FritzAndFriends upstream)

## Design Patterns Followed

- **Before/After Format:** Tabbed code blocks showing Web Forms vs. Blazor side-by-side (consistent with MasterPages.md)
- **Reference Tables:** Quick-lookup tables for lifecycle mapping and service lifetimes (improves scannability)
- **Direct Mapping Language:** "Replace X with Y" throughout (actionable for developers)
- **Audience-Aware Tone:** Acknowledges developers' Web Forms expertise; doesn't condescend

## Scope Covered

✅ `runat="server"`  
✅ `ViewState`  
✅ `UpdatePanel`  
✅ `ScriptManager` & JavaScript interop  
✅ PostBack events  
✅ Page lifecycle (`Page_Load`, `Page_Init`, etc.)  
✅ `IsPostBack` check pattern  
✅ Server-side control property manipulation  
✅ Page.Title → `<PageTitle>` component  
✅ `ItemDataBound` events  
✅ Application & Session state  
✅ Server-side event timing  
✅ Migration checklist (bonus)  

## Expected Impact

- **Accelerates migrations:** Developers no longer pause at unfamiliar patterns—guidance is one click away
- **Reduces support burden:** Common questions answered by canonical documentation
- **Improves discoverability:** Strategic nav placement ensures developers encounter this guide early
- **Future-proofs:** Serves as reference for all future Web Forms → Blazor migrations
