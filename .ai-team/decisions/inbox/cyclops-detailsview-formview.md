# DetailsView + FormView Polish Decisions

**By:** Cyclops
**Date:** Milestone 7

## WI-26: DetailsView Style Sub-Components

**What:** Created `IDetailsViewStyleContainer` interface with 10 style properties and 10 sub-component pairs following the established GridView/Calendar pattern. DetailsView has two extra styles vs GridView: `CommandRowStyle` (for the Edit/Delete/New command row) and `FieldHeaderStyle` (for the left-side header cell in each data row). `InsertRowStyle` is separate from `EditRowStyle` to match Web Forms semantics where Insert and Edit modes can be styled independently.

**Why:** Consistent with the GridView style sub-component architecture (WI-05). DetailsView has distinct row types (command row, field headers) that Web Forms styled separately. CascadingParameter name is "ParentDetailsView" to avoid collision with "ParentGridView".

## WI-28: DetailsView Caption + PagerSettings

**What:** Added `Caption` (string), `CaptionAlign` (TableCaptionAlign enum), and `PageCount` (computed int). Reuses existing `TableCaptionAlign` enum and `GetCaptionStyle()` pattern from GridView. `PageCount` is a read-only computed property (`Items.Count()`) since DetailsView shows one item per page. PagerSettings deferred to a future WI — the current implementation uses the existing PagerTemplate approach and inline numeric pager.

**Why:** Caption/CaptionAlign match GridView's implementation exactly. PageCount is trivially derived from the data source. Full PagerSettings (Mode, Position, PageButtonCount, navigation text) is better as a dedicated sub-component in a follow-up WI to keep this change focused.

## WI-31: FormView Remaining Events

**What:** Added `ModeChanged` (fires after mode transitions), `ItemCommand` (fires for all command bubbling via `FormViewCommandEventArgs`), `ItemCreated` (fires on first render), `PageIndexChanging`/`PageIndexChanged` (with cancellation via `PageChangedEventArgs.Cancel`). Added "page" command handler supporting "next"/"prev"/"first"/"last"/numeric arguments.

**Why:** Web Forms FormView fires ModeChanged after every mode switch. ItemCommand is the catch-all command handler that fires before specific handlers. ItemCreated maps to the initial data-bound lifecycle. Page events reuse the existing `PageChangedEventArgs` class (shared with GridView/DetailsView).

## WI-33: FormView Style Sub-Components + Pager + Caption

**What:** Created `IFormViewStyleContainer` interface with 7 style properties and 7 sub-component pairs. Added `PagerTemplate` (RenderFragment) that replaces the default numeric pager when set. Added `Caption`/`CaptionAlign` using the same pattern as DetailsView/GridView. `GetCurrentRowStyle()` resolves style based on `CurrentMode` (Edit→EditRowStyle, Insert→InsertRowStyle, default→RowStyle).

**Why:** FormView doesn't have AlternatingRowStyle because it only displays one item at a time. The 7 styles (RowStyle, EditRowStyle, InsertRowStyle, HeaderStyle, FooterStyle, EmptyDataRowStyle, PagerStyle) cover all distinct visual regions. PagerTemplate enables custom pager markup, matching Web Forms' `<PagerTemplate>` element.
