# GridView Display Properties — Decision Record

**Author:** Cyclops  
**Date:** WI-07 implementation  
**Component:** GridView

## Decisions

### 1. ShowHeaderWhenEmpty defaults to false (breaking behavior change)

Previously, GridView always rendered `<thead>` when columns existed, regardless of data. Now `ShouldRenderHeader = ShowHeader && (HasData || ShowHeaderWhenEmpty)`. With `ShowHeaderWhenEmpty=false` (default), the header is hidden when the data source is empty. This matches Web Forms behavior where `ShowHeaderWhenEmpty` was added in .NET 4.5 with default `false`.

**Impact:** Existing GridViews with empty data will stop showing headers unless `ShowHeaderWhenEmpty="true"` is added. One test (`EmptyDataText.razor`) was updated accordingly.

### 2. UseAccessibleHeader adds scope="col" to existing th elements

The current GridView already renders `<th>` in the header (not `<td>`). Rather than changing the default to `<td>` (which would be a larger breaking change), `UseAccessibleHeader=true` adds `scope="col"` to the existing `<th>` elements for accessibility compliance. When false (default), `<th>` renders without scope — preserving existing HTML output.

### 3. GridLines.None suppresses the rules attribute entirely

When `GridLines=None` (default), `GetGridLinesRules()` returns `null`, so Blazor omits the `rules` attribute from the `<table>` element. This matches Web Forms behavior where `GridLines.None` means no `rules` attribute is rendered.

### 4. CellPadding/CellSpacing use -1 sentinel for "don't render"

Following Web Forms convention, `-1` means the attribute is not rendered. Any value `>= 0` renders the corresponding `cellpadding`/`cellspacing` attribute on the `<table>`.

### 5. ShowFooter and paging share a single tfoot

When both `ShowFooter=true` and `AllowPaging` with multiple pages, both the footer row and pager row render inside the same `<tfoot>` element. The footer row renders first, followed by the pager row. Footer row gets `FooterStyle` applied.

### 6. EmptyDataTemplate takes precedence over EmptyDataText

When both `EmptyDataTemplate` (RenderFragment) and `EmptyDataText` (string) are set, the template wins. This matches Web Forms behavior.
