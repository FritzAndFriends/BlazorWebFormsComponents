# Decision: BWFC Control Preservation is Mandatory

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-03-05
**Status:** Decided
**Requested by:** Jeffrey T. Fritz

## Context

Jeff's directive: "We need to ALWAYS preserve the default asp: controls by using the BWFC components."

The AfterWingtipToys `ShoppingCart.razor` was migrated before the migration scripts existed. Someone (AI or human) decomposed the `<asp:GridView>` into a plain HTML `<table>` with `@foreach`, destroying editability, row stripes, footer totals, and making the cart read-only. The BWFC GridView supports ALL features needed — zero component gaps.

The migration script (`bwfc-migrate.ps1`) already does the right thing mechanically: `ConvertFrom-AspPrefix` strips `asp:` prefix, `Remove-WebFormsAttributes` converts `ItemType` → `TItem`, etc. The problem is Layer 2 (human/AI) work that rewrites controls as raw HTML.

## Decision

1. **ALL asp: controls MUST be preserved as BWFC components in migration output.** No exceptions.
2. **NEVER flatten data controls** (GridView, ListView, Repeater, DataList, DataGrid, DetailsView, FormView) to raw HTML tables or `@foreach` loops.
3. **NEVER flatten editor controls** (TextBox, CheckBox, Button, Label, etc.) to raw HTML `<input>`, `<button>`, `<span>` elements.
4. **NEVER flatten navigation/structural controls** (HyperLink, ImageButton, LinkButton, Panel, PlaceHolder, etc.) to raw HTML.
5. **Post-transform verification** (`Test-BwfcControlPreservation`) runs automatically after Layer 1 transforms and emits warnings for any control deficit.

## Implementation

### SKILL.md Update
- Added mandatory "BWFC Control Preservation" section with 5 rules, ShoppingCart anti-pattern, BAD vs GOOD examples, and rationale.

### Script Enhancement
- `Test-BwfcControlPreservation` function added to `bwfc-migrate.ps1`:
  - Counts asp: tags in source vs BWFC component tags in output
  - Warns on deficit with specific control names
  - Non-blocking: warnings in ManualItems report
  - Wired into `Convert-WebFormsFile` after all transforms, before file write

## Rationale

- The whole point of BWFC is that these components exist — use them
- BWFC components render identical HTML to Web Forms controls → CSS preservation
- Data controls have built-in sorting, paging, editing, footer totals → feature parity
- Preserving controls means 90% of markup is done after asp: prefix stripping → migration velocity
- ShoppingCart proves the cost: a "migrated" page that users can't actually use

## Affects

- `migration-toolkit/scripts/bwfc-migrate.ps1` — new `Test-BwfcControlPreservation` function
- `.ai-team/skills/migration-standards/SKILL.md` — new mandatory control preservation section
- All future Layer 2 migration work — must preserve BWFC components
- All team members performing migrations (human or AI)
