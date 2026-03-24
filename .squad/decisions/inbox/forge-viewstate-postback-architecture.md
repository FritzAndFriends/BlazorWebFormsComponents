# Decision: Enhanced ViewState & PostBack Shim Architecture

**By:** Forge (Lead / Web Forms Reviewer)  
**Date:** 2026-03-24  
**Status:** Proposed — Awaiting Jeffrey's Review

## What

Architecture proposal to upgrade ViewState and IsPostBack from compile-time stubs to working persistence mechanisms that auto-adapt to Blazor SSR and ServerInteractive rendering modes.

## Key Decisions

1. **ViewState becomes real:** Replace `Dictionary<string, object>` with `ViewStateDictionary`. In SSR mode, round-trips via Data Protection-encrypted hidden form field. In Interactive mode, persists in component instance memory (already works). Remove `[Obsolete]`.

2. **IsPostBack becomes mode-adaptive:** SSR → returns `true` on HTTP POST, `false` on GET. Interactive → returns `false` during OnInitialized, `true` on subsequent renders. Remove hardcoded `false`.

3. **AutoPostBack gets real behavior:** SSR → emits `onchange="this.form.submit()"` on controls. Interactive → existing Blazor `@onchange` is already equivalent. Remove `[Obsolete]`.

4. **IPostBackEventHandler NOT shimmed:** Too deep in Web Forms plumbing. BWFC023 analyzer continues recommending `EventCallback<T>`.

5. **Analyzer updates:** BWFC002/003 severity reduced to Info. BWFC020 changed to Suggestion. BWFC023 unchanged. New BWFC025 for non-serializable ViewState types.

6. **Auto-detection, no configuration:** Uses existing `HttpContext` availability pattern. No explicit render mode configuration needed.

## Why

ASCX user control migration is a primary use case. The DepartmentFilter pattern (ViewState-backed properties + `!IsPostBack` guard) is universal in Web Forms codebases. Making these shims work means code-behind files migrate with zero changes — only markup needs updating.

## Impact

- `BaseWebFormsComponent.ViewState` type changes from `Dictionary<string, object>` to `ViewStateDictionary` (implements `IDictionary<string, object>`, backward compatible)
- `WebFormsPageBase.IsPostBack` changes from `=> false` to mode-adaptive property
- `[Obsolete]` removed from ViewState, IsPostBack, and AutoPostBack
- 4 analyzers updated (BWFC002, BWFC003, BWFC020 messages; new BWFC025)
- New dependency: `Microsoft.AspNetCore.DataProtection` (for ViewState encryption in SSR)

## Estimated Effort

7 weeks across 5 phases: Core Infrastructure (2w), SSR Persistence (2w), AutoPostBack (1w), Analyzers (1w), Docs & Samples (1w).

## Reference

Full proposal: `dev-docs/architecture/ViewState-PostBack-Shim-Proposal.md`
