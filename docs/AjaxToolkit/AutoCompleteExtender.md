# AutoCompleteExtender

The **AutoCompleteExtender** provides typeahead/autocomplete functionality for a target TextBox. As the user types, it fetches suggestions and renders a dropdown list with keyboard navigation support. Suggestions can be fetched via a service URL or a Blazor `EventCallback`.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/AutoCompleteExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the TextBox to enhance with autocomplete
- `ServicePath` — URL of the web service providing completion data
- `ServiceMethod` — Web service method name for completion data
- `MinimumPrefixLength` — Minimum characters before suggestions are fetched
- `CompletionSetCount` — Maximum number of suggestions to display
- `CompletionInterval` — Milliseconds between keystrokes before fetching
- `CompletionListCssClass` — CSS class for the dropdown container
- `CompletionListItemCssClass` — CSS class for each dropdown item
- `CompletionListHighlightedItemCssClass` — CSS class for the highlighted item
- `DelimiterCharacters` — Characters that delimit separate entries in the TextBox
- `FirstRowSelected` — Whether to auto-select the first suggestion
- `ShowOnlyCurrentWordInCompletionListItem` — Show only the current word in suggestions
- `OnClientItemSelected` — Script invoked when an item is selected
- `OnClientPopulating` — Script invoked before the list is populated
- `OnClientPopulated` — Script invoked after the list is populated
- `CompletionDataRequested` — Blazor callback for server-side suggestion fetching
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Syntax

```html
<asp:TextBox ID="txtSearch" runat="server" />

<ajaxToolkit:AutoCompleteExtender
    ID="ace1"
    runat="server"
    TargetControlID="txtSearch"
    ServicePath="~/Services/SearchService.asmx"
    ServiceMethod="GetSuggestions"
    MinimumPrefixLength="2"
    CompletionSetCount="10"
    CompletionInterval="500"
    CompletionListCssClass="autocomplete-list"
    CompletionListItemCssClass="autocomplete-item"
    CompletionListHighlightedItemCssClass="autocomplete-highlight"
    FirstRowSelected="true" />
```

## Blazor Migration

```razor
<TextBox ID="txtSearch" />

<AutoCompleteExtender
    TargetControlID="txtSearch"
    ServicePath="/api/search"
    ServiceMethod="GetSuggestions"
    MinimumPrefixLength="2"
    CompletionSetCount="10"
    CompletionInterval="500"
    CompletionListCssClass="autocomplete-list"
    CompletionListItemCssClass="autocomplete-item"
    CompletionListHighlightedItemCssClass="autocomplete-highlight"
    FirstRowSelected="true" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Update the `ServicePath` to your new API endpoint. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the TextBox to enhance with autocomplete |
| `ServicePath` | `string` | `""` | URL of the web service providing completion data |
| `ServiceMethod` | `string` | `""` | Web service method name to call for completion data |
| `MinimumPrefixLength` | `int` | `3` | Minimum characters typed before suggestions are fetched |
| `CompletionSetCount` | `int` | `10` | Maximum number of suggestions to display |
| `CompletionInterval` | `int` | `1000` | Time in milliseconds between keystrokes before fetching suggestions |
| `CompletionListCssClass` | `string` | `""` | CSS class applied to the completion list dropdown container |
| `CompletionListItemCssClass` | `string` | `""` | CSS class applied to each item in the completion list |
| `CompletionListHighlightedItemCssClass` | `string` | `""` | CSS class applied to the highlighted item |
| `DelimiterCharacters` | `string` | `""` | Characters that delimit separate completion entries |
| `FirstRowSelected` | `bool` | `false` | Whether to auto-select the first item in the completion list |
| `ShowOnlyCurrentWordInCompletionListItem` | `bool` | `false` | Show only the current word in each completion item |
| `OnClientItemSelected` | `string` | `""` | Client-side script invoked when an item is selected |
| `OnClientPopulating` | `string` | `""` | Client-side script invoked before the list is populated |
| `OnClientPopulated` | `string` | `""` | Client-side script invoked after the list is populated |
| `CompletionDataRequested` | `EventCallback<string>` | — | Blazor callback for requesting completion data server-side |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Autocomplete with Service

```razor
@rendermode InteractiveServer

<TextBox ID="txtCity" />

<AutoCompleteExtender
    TargetControlID="txtCity"
    ServicePath="/api/cities"
    ServiceMethod="Search"
    MinimumPrefixLength="2"
    CompletionSetCount="10" />
```

### Autocomplete with Styled Dropdown

```razor
@rendermode InteractiveServer

<TextBox ID="txtProduct" />

<AutoCompleteExtender
    TargetControlID="txtProduct"
    ServicePath="/api/products"
    ServiceMethod="Suggest"
    MinimumPrefixLength="1"
    CompletionSetCount="8"
    CompletionInterval="300"
    CompletionListCssClass="ac-dropdown"
    CompletionListItemCssClass="ac-item"
    CompletionListHighlightedItemCssClass="ac-item-highlight"
    FirstRowSelected="true" />

<style>
    .ac-dropdown {
        border: 1px solid #ccc; background: white;
        max-height: 200px; overflow-y: auto; box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    }
    .ac-item { padding: 8px 12px; cursor: pointer; }
    .ac-item-highlight { background: #007bff; color: white; }
</style>
```

### Multi-Value Autocomplete with Delimiters

```razor
@rendermode InteractiveServer

<TextBox ID="txtTags" />

<AutoCompleteExtender
    TargetControlID="txtTags"
    ServicePath="/api/tags"
    ServiceMethod="Search"
    MinimumPrefixLength="1"
    DelimiterCharacters=",;"
    ShowOnlyCurrentWordInCompletionListItem="true" />
```

### Autocomplete with JavaScript Callbacks

```razor
@rendermode InteractiveServer

<TextBox ID="txtEmployee" />

<AutoCompleteExtender
    TargetControlID="txtEmployee"
    ServicePath="/api/employees"
    ServiceMethod="Find"
    MinimumPrefixLength="2"
    OnClientItemSelected="onEmployeeSelected"
    OnClientPopulating="onSearchStart"
    OnClientPopulated="onSearchEnd" />

<script>
    function onEmployeeSelected(sender, args) {
        console.log('Selected:', args);
    }
    function onSearchStart() {
        console.log('Searching...');
    }
    function onSearchEnd() {
        console.log('Search complete');
    }
</script>
```

## HTML Output

The AutoCompleteExtender produces no HTML itself — it attaches JavaScript behavior to the target TextBox. When suggestions are fetched, the JavaScript module creates and manages a dropdown list in the DOM.

## JavaScript Interop

The AutoCompleteExtender loads `autocomplete-extender.js` as an ES module. JavaScript handles:

- Monitoring keystrokes and debouncing requests
- Fetching suggestions from the configured service endpoint
- Rendering and positioning the dropdown list
- Keyboard navigation (Up/Down arrows, Enter to select, Escape to close)
- Highlighting the selected item
- Delimiter-aware text insertion

## Render Mode Requirements

The AutoCompleteExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The TextBox works as a plain text input.
- **JavaScript disabled:** Same as SSR — TextBox functions without autocomplete.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:AutoCompleteExtender
   + <AutoCompleteExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Update ServicePath** — Change from .asmx paths to your new API endpoints
   ```diff
   - ServicePath="~/Services/SearchService.asmx"
   + ServicePath="/api/search"
   ```

4. **Consider using `CompletionDataRequested`** — For Blazor-native data fetching instead of web services

### Before (Web Forms)

```html
<asp:TextBox ID="txtSearch" runat="server" />

<ajaxToolkit:AutoCompleteExtender
    ID="ace1"
    TargetControlID="txtSearch"
    ServicePath="~/SearchService.asmx"
    ServiceMethod="GetSuggestions"
    MinimumPrefixLength="2"
    CompletionSetCount="10"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtSearch" />

<AutoCompleteExtender
    TargetControlID="txtSearch"
    ServicePath="/api/search"
    ServiceMethod="GetSuggestions"
    MinimumPrefixLength="2"
    CompletionSetCount="10" />
```

## Best Practices

1. **Set appropriate MinimumPrefixLength** — Too low (1) causes excessive requests; too high (5) feels unresponsive
2. **Tune CompletionInterval** — 300–500ms is a good balance between responsiveness and server load
3. **Limit CompletionSetCount** — 8–12 items keeps the dropdown manageable
4. **Style the dropdown** — Use CSS classes to match your application's design
5. **Handle empty results gracefully** — Consider showing a "No results" message

## Troubleshooting

| Issue | Solution |
|---|---|
| No suggestions appearing | Verify `ServicePath` and `ServiceMethod` are correct and returning data. Check `MinimumPrefixLength` isn't too high. Ensure `@rendermode InteractiveServer` is set. |
| Suggestions appear slowly | Reduce `CompletionInterval` (e.g., 300ms). Check server response times. |
| Wrong item highlighted | Verify `CompletionListHighlightedItemCssClass` is set and has visible styling. |
| Delimiter mode not working | Ensure `DelimiterCharacters` contains the correct separator characters. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [FilteredTextBoxExtender](FilteredTextBoxExtender.md) — Character filtering for text input
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
