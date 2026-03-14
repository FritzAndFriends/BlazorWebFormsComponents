# UpdatePanel ContentTemplate Documentation Update

**Owner:** Beast (Technical Writer)  
**Date:** 2026-03-12  
**Related:** UpdatePanel component enhancement (L1 migration now supports ContentTemplate cleanly)

## Decision

Updated `migration-toolkit/skills/bwfc-migration/CONTROL-REFERENCE.md` to document UpdatePanel's new `ContentTemplate` RenderFragment support.

## Background

The UpdatePanel component now supports a `ContentTemplate` RenderFragment parameter. This enables L1 migration to convert Web Forms `<asp:UpdatePanel>` with `<ContentTemplate>` child elements to clean Blazor markup without RZ10012 ("unknown element") warnings.

## Change Summary

**File updated:** `CONTROL-REFERENCE.md` (AJAX Controls section)

1. **Table entry:** Expanded UpdatePanel row to note `<ContentTemplate>` support
2. **New subsection:** Added "UpdatePanel with ContentTemplate" documenting:
   - Before/after code examples (Web Forms → Blazor)
   - Key points: ContentTemplate recognition, BaseStyledComponent inheritance, render mode placement
   - Clarification that render mode is set at app level via `App.razor`, not by UpdatePanel

## No Changes Needed

- `CODE-TRANSFORMS.md` — no ContentTemplate-specific code transforms
- `SKILL.md` — no special UpdatePanel migration notes
- `migration-standards/SKILL.md` — no ContentTemplate warnings to update
- `bwfc-data-migration/SKILL.md` — no UpdatePanel references
- `bwfc-identity-migration/SKILL.md` — ContentTemplate mentioned in context (no update needed)

## Rationale

**Surgical update approach:** Only CONTROL-REFERENCE.md needed changes. This is the canonical control translation reference table, so documenting new capabilities here ensures developers migrating Web Forms apps see the capability immediately when consulting the table.

## Impact

- Developers migrating Web Forms `<asp:UpdatePanel>` with `<ContentTemplate>` can now proceed confidently with L1 output
- No breaking changes — this is purely additive documentation
- Aligns with improved component capability (BaseStyledComponent, render fragments)

## References

- Component: `BlazorWebFormsComponents.UpdatePanel`
- Related migration skill: `bwfc-migration`
- L1 migration script: `migration-toolkit/scripts/bwfc-migrate.ps1`
