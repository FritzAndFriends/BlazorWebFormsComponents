# L1 Migration Script Test Harness

Quality measurement suite for `bwfc-migrate.ps1` (Layer 1 mechanical transforms).

## Quick Start

```powershell
.\Run-L1Tests.ps1          # Run all tests
.\Run-L1Tests.ps1 -Verbose # Show detailed per-file logging
```

## Structure

```
tests/
├── inputs/          # .aspx test case files (one transform per file)
├── expected/        # .razor expected output files
├── Run-L1Tests.ps1  # Test runner with metrics reporting
└── README.md
```

## Test Cases

| ID | Category | Tests |
|----|----------|-------|
| TC01 | Markup — asp: prefix removal | `<asp:Button>` → `<Button>` |
| TC02 | Attributes — WebForms attribute stripping | runat, EnableViewState, ViewStateMode, etc. |
| TC03 | Directives — Page directive + title extraction | `<%@ Page Title="..." %>` → `@page` + `<PageTitle>` |
| TC04 | Directives — Import namespace conversion | `<%@ Import %>` → `@using` |
| TC05 | Layout — form wrapper to div | `<form runat="server">` → `<div>` |
| TC06 | Expressions — data binding + output expressions | Eval, Item, `<%: %>`, `<%= %>` |
| TC07 | URLs — tilde path normalization | `~/path` → `/path` |
| TC08 | Comments — server comment conversion | `<%-- --%>` → `@* *@` |
| TC09 | Content — asp:Content wrapper removal | Strips wrapper, preserves inner content |
| TC10 | Types — ItemType conversion + auto-add | `ItemType="NS.Class"` → `TItem="Class"` |

## Adding Test Cases

1. Create `inputs/TC##-Name.aspx` with a minimal Web Forms file testing ONE transform
2. Create `expected/TC##-Name.razor` with the correct expected Blazor output
3. Run `.\Run-L1Tests.ps1` — the new test is auto-discovered

## Metrics Reported

- **Pass rate** — test cases producing exact expected output
- **Line accuracy** — matching lines / total lines across all tests
- **Per-file timing** — milliseconds per transformation
- **Diff details** — line-by-line expected vs actual for failures

## Baseline Results (2026-07-25)

```
Pass rate:       7 / 10 (70%)
Line accuracy:   50 / 53 (94.3%)
Avg per file:    114ms
```

### Known Failures

1. **TC06 Eval expression** — `<%#: Eval("Name") %>` partially converted to `<%#: context.Name %>` instead of `@context.Name`. The Eval regex replaces the inner call but the `<%#:` / `%>` delimiters survive.
2. **TC09 Content indentation** — First content line after `<asp:Content>` loses leading whitespace. The opening tag regex `\s*\r?\n?` consumes the indent of the next line.
3. **TC10 ItemType double-add** — Components with explicit `ItemType` (converted to `TItem`) also get `ItemType="object"` fallback added because the negative lookahead checks for `ItemType=` after it was already renamed to `TItem=`.
