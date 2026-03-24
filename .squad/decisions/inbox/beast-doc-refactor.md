# Documentation Refactor Decisions

**Status:** APPROVED  
**Date:** 2026-03-XX  
**Owner:** Beast (Technical Writer)  
**Context:** Documentation audit completed identifying 6 critical improvements

## Decision 1: Landing Page Expansion Strategy

**Problem:** docs/README.md was ~12 lines of brief descriptors, missing new developer context.

**Decision:** Restructure as 7-section landing page (~70 lines) with narrative flow from "what is this" → "who should use it" → "how to start" → "what's included" → "where to go next"

**Rationale:** 
- Developers visiting the site need immediate clarity: Is this for me? How do I start? 
- Root README points to docs/README.md as primary entry point
- Current format lacks onboarding experience
- Sections follow discovery-to-action pattern: value prop → audience targeting → quick start → resources

**Implementation:**
- Section 1: "What is BlazorWebFormsComponents?" — Concise technical definition + compatibility promise
- Section 2: "Who This Is For" — Brown-field developers maintaining Web Forms apps
- Section 3: "Quick Start" — 4-step path: Install → Register → Import → Use
- Section 4: "Component Overview" — 7 categories with quick links
- Section 5: "Migration Resources" — Hierarchical guides + status tracking
- Section 6: "Key Features" — Bullet-point value props (same names, same markup, etc.)
- Section 7: "What You Need to Know" — Compatibility caveats + framework requirements

**Cost:** ~50 tokens of prose (estimated); One edit to existing file

## Decision 2: Tabbed Syntax Pattern for Component Docs

**Problem:** 25+ component docs have separate "Web Forms Declarative Syntax" and "Blazor Syntax" sections, making side-by-side comparison hard.

**Decision:** Adopt `=== "Web Forms"` / `=== "Blazor"` tabbed blocks using `pymdownx.tabbed` extension (already enabled in mkdocs.yml).

**Rationale:**
- pymdownx.tabbed already configured → zero setup cost
- Tabs make before/after migration patterns immediately visible
- Better UX: readers can compare syntax without scrolling
- Pattern aligns with AjaxToolkit docs (best-documented category)
- Scalable: can be applied to 25+ component files systematically

**Implementation:**
- Replace "## Web Forms Declarative Syntax" + "## Blazor Syntax" sections with single "## Syntax" section
- Web Forms block shows full attribute documentation
- Blazor block shows simplified property-based syntax + brief explanation
- Examples section follows (no longer nested in Blazor tab)

**Files Converted (MVP):**
- Button.md, Panel.md, CheckBox.md (4 total)

**Future Conversion:**
- DropDownList.md, TextBox.md, ListBox.md (data-binding patterns)
- ValidationSummary.md, Label.md, RequiredFieldValidator.md (stub expansion opportunity)
- DataGrid.md, GridView.md, Repeater.md (template rendering)

**Cost:** ~30 minutes per file (identify blocks, reformat tabs, no content changes)

## Decision 3: Admonition Integration for Migration Guidance

**Problem:** Component docs lack contextual warnings about breaking changes, migration patterns, and browser limitations.

**Decision:** Adopt `!!! tip`, `!!! note`, `!!! warning` admonitions (MkDocs native, already enabled) at strategic locations:
- `!!! tip` — Blazor-specific patterns that improve on Web Forms (e.g., @bind-Checked replaces AutoPostBack)
- `!!! note` — Rendering behavior differences that affect CSS/JS (e.g., GroupingText creates <fieldset>)
- `!!! warning` — Security or unsupported features (e.g., OnClientClick injection risks)

**Rationale:**
- AjaxToolkit docs demonstrate this pattern effectively
- Admonitions draw reader attention without disrupting flow
- Signals migration gotchas early (prevents post-migration surprises)
- mkdocs.yml already has admonition extension enabled

**Placement Strategy:**
- After syntax tabs, before or within Examples section
- One admonition per conceptual concern (not excessive)
- Link to "See Also" or related docs when applicable

**Files Converted:**
- Button.md: Command event bubbling (tip), OnClientClick security (warning)
- Panel.md: Semantic HTML rendering (note), ScrollBars overflow (note)
- CheckBox.md: Two-way binding replaces AutoPostBack (tip), disabled styling limitations (note)

**Cost:** ~5 minutes per admonition (short prose)

## Decision 4: Strategies.md Link Placeholder Fix

**Problem:** Strategies.md line 23 contains `**INSERT LINK**` placeholder referencing _Imports.razor sample.

**Decision:** Replace with actual relative link: `/samples/AfterBlazorServerSide/Components/_Imports.razor`

**Rationale:**
- Sample projects are definitive source for _Imports.razor patterns
- Link provides working example developers can copy
- Maintains consistency with other docs linking to samples

**Files Modified:**
- docs/Migration/Strategies.md (1 inline replacement)

**Cost:** ~2 minutes (single find/replace)

## Decision 5: Template Patterns for Future Work

**Output for Future Refactoring Sessions:**

### Tabbed Syntax Template
```markdown
## Syntax

=== "Web Forms"

    ```html
    <!-- Full attribute documentation here -->
    ```

=== "Blazor"

    ```razor
    <!-- Simplified property-based syntax -->
    ```

## Examples
```

### Admonition Placement Template
```markdown
### [Feature Name]

!!! [tip|note|warning] "[Title]"
    Brief explanation

```razor
<!-- Code example -->
```
```

### Quick Start Link Pattern (for landing pages)
```markdown
## Quick Start

1. **Install:** `dotnet add package ...`
2. **Register:** Add to Program.cs
3. **Import:** Add to _Imports.razor
4. **Use:** Replace `<asp:` with `<` (remove prefix + runat)
```

## Implementation Timeline

**Phase 1 (Complete):** Landing page + 4 component conversions  
**Phase 2 (Recommended):** DropDownList, TextBox, ListBox + stub expansion  
**Phase 3 (Nice-to-have):** Full conversion of 25+ components to tabbed syntax  

## Success Criteria

✅ docs/README.md expanded to 60–80 lines with clear navigation  
✅ At least 4 component files converted to tabbed syntax  
✅ Admonitions in place with consistent style  
✅ Strategies.md link functional  
✅ mkdocs build succeeds; no broken links  
✅ Patterns documented for future writers (this file)

## Notes

- mkdocs.yml `pymdownx.tabbed` uses `alternate_style: true` (modern Material Design tabs)
- Admonition titles are optional but recommended for context
- All decisions preserve existing content; no deletions beyond reorganization
- Patterns designed to be scalable across component categories
