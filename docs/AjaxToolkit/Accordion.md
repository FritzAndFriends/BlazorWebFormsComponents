# Accordion

The **Accordion** displays a set of collapsible content panes arranged vertically. Only one pane is expanded at a time, allowing users to navigate between sections without leaving the page. The companion **AccordionPane** component defines each header/content pair within the Accordion.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/Accordion

## Features Supported in Blazor

- `SelectedIndex` — Zero-based index of the currently expanded pane
- `SelectedIndexChanged` — Event raised when the selected pane changes
- `FadeTransitions` — Whether to use fade transitions when switching panes
- `TransitionDuration` — Duration of expand/collapse animations in milliseconds
- `HeaderCssClass` — CSS class applied to each pane header div
- `ContentCssClass` — CSS class applied to each pane content div
- `RequireOpenedPane` — Whether at least one pane must always be open
- `AutoSize` — How pane content areas are automatically sized
- `SuppressHeaderPostbacks` — Whether header clicks suppress postback behavior
- `CssClass` — CSS class applied to the outer Accordion container div
- `ChildContent` — Contains `AccordionPane` child components

### AccordionPane Parameters

- `Header` — RenderFragment for the clickable header content
- `Content` — RenderFragment for the collapsible body content

## AutoSize Enum

Controls how pane content areas are sized:

```csharp
enum AutoSizeMode
{
    None = 0,   // No automatic sizing (default)
    Fill = 1,   // Content areas fill available space
    Limit = 2   // Content areas are limited to a maximum size
}
```

## Web Forms Syntax

```html
<ajaxToolkit:Accordion
    ID="acc1"
    runat="server"
    SelectedIndex="0"
    FadeTransitions="true"
    TransitionDuration="300"
    HeaderCssClass="accordionHeader"
    ContentCssClass="accordionContent"
    RequireOpenedPane="true"
    AutoSize="None"
    SuppressHeaderPostbacks="true">
    <Panes>
        <ajaxToolkit:AccordionPane>
            <Header>Section 1</Header>
            <Content>
                <p>Content for section 1.</p>
            </Content>
        </ajaxToolkit:AccordionPane>
        <ajaxToolkit:AccordionPane>
            <Header>Section 2</Header>
            <Content>
                <p>Content for section 2.</p>
            </Content>
        </ajaxToolkit:AccordionPane>
    </Panes>
</ajaxToolkit:Accordion>
```

## Blazor Migration

```razor
<Accordion
    SelectedIndex="0"
    FadeTransitions="true"
    TransitionDuration="300"
    HeaderCssClass="accordionHeader"
    ContentCssClass="accordionContent"
    RequireOpenedPane="true"
    AutoSize="AutoSizeMode.None"
    SuppressHeaderPostbacks="true">
    <AccordionPane>
        <Header>Section 1</Header>
        <Content>
            <p>Content for section 1.</p>
        </Content>
    </AccordionPane>
    <AccordionPane>
        <Header>Section 2</Header>
        <Content>
            <p>Content for section 2.</p>
        </Content>
    </AccordionPane>
</Accordion>
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix, remove `runat="server"` and `ID`, and replace the `<Panes>` wrapper with direct `<AccordionPane>` children. Everything else stays the same!

## Properties Reference

### Accordion

| Property | Type | Default | Description |
|---|---|---|---|
| `SelectedIndex` | `int` | `0` | Zero-based index of the currently expanded pane. Set to -1 for no expanded pane (only valid when `RequireOpenedPane` is false) |
| `SelectedIndexChanged` | `EventCallback<int>` | — | Raised when the selected pane index changes |
| `FadeTransitions` | `bool` | `false` | Whether to use fade transitions when switching panes |
| `TransitionDuration` | `int` | `300` | Duration of expand/collapse animations in milliseconds |
| `HeaderCssClass` | `string` | `""` | CSS class applied to each pane header div |
| `ContentCssClass` | `string` | `""` | CSS class applied to each pane content div |
| `RequireOpenedPane` | `bool` | `true` | Whether at least one pane must always be open |
| `AutoSize` | `AutoSizeMode` | `None` | How pane content areas are automatically sized |
| `SuppressHeaderPostbacks` | `bool` | `true` | Whether header clicks suppress postback behavior |
| `CssClass` | `string` | `""` | CSS class applied to the outer Accordion container div |

### AccordionPane

| Property | Type | Default | Description |
|---|---|---|---|
| `Header` | `RenderFragment` | — | The header content rendered inside the clickable header div |
| `Content` | `RenderFragment` | — | The body content rendered inside the collapsible content div |

## Usage Examples

### Basic Accordion

```razor
@rendermode InteractiveServer

<Accordion SelectedIndex="0" HeaderCssClass="acc-header" ContentCssClass="acc-content">
    <AccordionPane>
        <Header>Getting Started</Header>
        <Content>
            <p>Welcome! This section covers the basics.</p>
        </Content>
    </AccordionPane>
    <AccordionPane>
        <Header>Advanced Topics</Header>
        <Content>
            <p>Dig deeper into advanced features.</p>
        </Content>
    </AccordionPane>
    <AccordionPane>
        <Header>FAQ</Header>
        <Content>
            <p>Frequently asked questions and answers.</p>
        </Content>
    </AccordionPane>
</Accordion>

<style>
    .acc-header {
        background: #333; color: white; padding: 10px 15px;
        border-bottom: 1px solid #555; font-weight: bold;
    }
    .acc-content { padding: 15px; background: #f9f9f9; }
</style>
```

### Accordion with Fade Transitions

```razor
@rendermode InteractiveServer

<Accordion
    SelectedIndex="0"
    FadeTransitions="true"
    TransitionDuration="500"
    HeaderCssClass="fade-header"
    ContentCssClass="fade-content">
    <AccordionPane>
        <Header>📋 Details</Header>
        <Content><p>Detailed information here.</p></Content>
    </AccordionPane>
    <AccordionPane>
        <Header>📊 Statistics</Header>
        <Content><p>Key metrics and data.</p></Content>
    </AccordionPane>
</Accordion>
```

### Accordion with All-Collapsed Support

When `RequireOpenedPane` is `false`, clicking the selected pane header collapses it:

```razor
@rendermode InteractiveServer

<Accordion
    SelectedIndex="-1"
    RequireOpenedPane="false"
    HeaderCssClass="toggle-header"
    ContentCssClass="toggle-content"
    SelectedIndexChanged="OnPaneChanged">
    <AccordionPane>
        <Header>Optional Section A</Header>
        <Content><p>This pane can be collapsed.</p></Content>
    </AccordionPane>
    <AccordionPane>
        <Header>Optional Section B</Header>
        <Content><p>This pane can also be collapsed.</p></Content>
    </AccordionPane>
</Accordion>

<p>Selected index: @currentIndex</p>

@code {
    private int currentIndex = -1;

    void OnPaneChanged(int index)
    {
        currentIndex = index;
    }
}
```

## HTML Output

The Accordion renders an outer `<div>` container with class `ajax__accordion` plus any custom `CssClass`. Each AccordionPane renders:

```html
<div id="..." class="ajax__accordion accordionClass">
    <!-- Pane 1 -->
    <div id="..." class="ajax__accordion_header headerClass" role="tab"
         aria-selected="true" aria-expanded="true" style="cursor:pointer;">
        Section 1
    </div>
    <div id="..." class="ajax__accordion_content contentClass" role="tabpanel">
        <p>Content for section 1.</p>
    </div>

    <!-- Pane 2 (collapsed) -->
    <div id="..." class="ajax__accordion_header headerClass" role="tab"
         aria-selected="false" aria-expanded="false" style="cursor:pointer;">
        Section 2
    </div>
    <div id="..." class="ajax__accordion_content contentClass" role="tabpanel"
         style="display:none;overflow:hidden;">
        <p>Content for section 2.</p>
    </div>
</div>
```

## JavaScript Interop

The Accordion loads `accordion.js` as an ES module on first render. JavaScript handles:

- Smooth slide/fade animations when switching panes
- `initAccordion()` — Initializes the accordion with configuration options
- `selectPane()` — Animates the transition between panes
- `disposeAccordion()` — Cleans up resources on disposal

## Render Mode Requirements

The Accordion requires **InteractiveServer** render mode for animated transitions:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The Accordion renders the correct HTML structure (selected pane visible, others hidden), but animated transitions won't work. Clicking headers still toggles panes via Blazor server events.
- **JavaScript disabled:** Same as SSR — pane switching works but without animation.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:Accordion
   + <Accordion
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Remove `<Panes>` wrapper** — Place `<AccordionPane>` elements directly inside `<Accordion>`

4. **Update enum references** — Use `AutoSizeMode.None` instead of `"None"`

### Before (Web Forms)

```html
<ajaxToolkit:Accordion ID="acc1" runat="server"
    SelectedIndex="0" RequireOpenedPane="true"
    HeaderCssClass="header" ContentCssClass="content">
    <Panes>
        <ajaxToolkit:AccordionPane>
            <Header>Section 1</Header>
            <Content><p>Content 1</p></Content>
        </ajaxToolkit:AccordionPane>
    </Panes>
</ajaxToolkit:Accordion>
```

### After (Blazor)

```razor
<Accordion SelectedIndex="0" RequireOpenedPane="true"
    HeaderCssClass="header" ContentCssClass="content">
    <AccordionPane>
        <Header>Section 1</Header>
        <Content><p>Content 1</p></Content>
    </AccordionPane>
</Accordion>
```

## Best Practices

1. **Use descriptive headers** — Help users scan section titles quickly
2. **Keep pane content focused** — Each pane should cover one topic or section
3. **Provide visual feedback** — Style active headers differently from inactive ones
4. **Consider default selection** — Start with the most relevant pane expanded
5. **Accessibility** — Headers use `role="tab"` and `aria-expanded` automatically

## Troubleshooting

| Issue | Solution |
|---|---|
| No panes visible | Verify `SelectedIndex` is within range (0 to pane count - 1). Check that AccordionPane components are direct children of Accordion. |
| Animation not working | Ensure `@rendermode InteractiveServer` is set. Check browser console for JavaScript errors. |
| All panes collapsed unexpectedly | If `RequireOpenedPane="false"`, clicking the active pane collapses it. Set `RequireOpenedPane="true"` if one pane must always be open. |
| AccordionPane throws exception | AccordionPane must be inside an Accordion component. Verify the nesting structure. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [CollapsiblePanelExtender](CollapsiblePanelExtender.md) — Single panel collapse/expand
- [TabContainer](TabContainer.md) — Tabbed content panels
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
