# Bishop — Cycle 1 Fix Decisions

**Date:** 2026-03-06
**By:** Bishop (Migration Tooling Dev)

## Decisions Made

### ItemType vs TItem: Control-Specific Conversion
- Only `DropDownList`, `ListBox`, `CheckBoxList`, `RadioButtonList`, `BulletedList` get `ItemType→TItem` conversion.
- All other data controls (`GridView`, `ListView`, `FormView`, `DetailsView`, `DataGrid`, `DataList`, `Repeater`) retain `ItemType` as-is.
- The regex uses `Singleline` mode to handle multi-line tag declarations.

### Smart Stub: Markup Always Transforms, Code-Behind Stubs
- Unconvertible pages (Account/*, Checkout/*) now receive ALL Layer 1 markup transforms.
- Only code-behinds are stubbed with a minimal partial class + TODO banner.
- This recovers ~20 pages of mechanical transforms that were previously wasted.
- Layer 2 no longer needs to redo markup transforms for these pages.

### Base Class Stripping in Code-Behinds
- `Copy-CodeBehind` now strips `: Page`, `: System.Web.UI.Page`, `: UserControl`, `: MasterPage` and all `using System.Web.*` directives.
- This avoids CS0263 "partial declarations must not specify different base classes" when `@inherits WebFormsPageBase` is set in `_Imports.razor`.

### Validator Type Parameters Default to String
- All validators get `Type="string"` or `InputType="string"` by default since most Web Forms apps validate strings.
- If a project needs different types, Layer 2 can adjust. But `string` is safe and eliminates ~26 build errors.

## Impact for Run 10
- Expect 0 CS0246/CS0535 errors from ItemType/TItem mismatch
- Expect 0 CS0263 errors from base class conflicts
- Expect ~20 more pages with pre-converted BWFC markup
- Expect ~26 fewer validator-related build errors
- ImageButton→img regressions will be caught and flagged
