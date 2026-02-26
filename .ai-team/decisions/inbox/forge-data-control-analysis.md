# Decision: Data Control Divergence Analysis Results

**Date:** 2026-02-27
**By:** Forge (Lead / Web Forms Reviewer)
**Status:** Pending Review
**Relates to:** M13 HTML Fidelity Audit — Data Controls (DataList, GridView, ListView, Repeater)

## Context

The M13 audit captured WebForms and Blazor HTML for 4 data controls showing 389 total line differences (DataList 110, GridView 33, ListView 182, Repeater 64). A line-by-line analysis was performed to classify each divergence.

## Decision

### 1. Sample Parity is the dominant cause of divergences

The majority of differences (estimated 90%+) are caused by the Blazor sample pages using different templates, styles, columns, and data formats than the corresponding Web Forms samples. The Blazor samples must be rewritten to match the Web Forms samples before meaningful fidelity comparison is possible.

**Implication:** Items 6–10 in the action plan (Jubilee tasks) are prerequisites for accurate audit results.

### 2. Five genuine component bugs identified

| Bug | Control | Description | Priority |
|-----|---------|-------------|----------|
| BUG-DL-2 | DataList | Missing `itemtype` attribute | P2 |
| BUG-DL-3 | DataList | `border-collapse:collapse` unconditionally rendered | P1 |
| BUG-GV-1 | GridView | Default GridLines may not match WF default (Both) | P1 |
| BUG-GV-2 | GridView | Missing default `border-collapse:collapse` | P1 |
| BUG-GV-3 | GridView | Empty `<th>` instead of `&nbsp;` for blank headers | P2 |

### 3. ListView and Repeater have zero component bugs

All differences in these two controls are entirely sample parity issues. The components render templates correctly.

### 4. Normalization pipeline gaps

- Blazor output for data controls has not been normalized (directories missing under `audit-output/normalized/blazor/`)
- `<!--!-->` Blazor rendering markers need to be stripped by the normalization pipeline (new rule needed)

## Full Analysis

See `planning-docs/DATA-CONTROL-ANALYSIS.md` for the complete line-by-line breakdown.

## Impact

- 3 P1 bugs need Cyclops fixes before M13 completion
- 4 sample rewrites needed (Jubilee) before re-capture
- Normalization pipeline update needed (Colossus)
