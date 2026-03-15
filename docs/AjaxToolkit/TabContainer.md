# TabContainer

The **TabContainer** displays content in tabbed panels, where only the active tab's content is visible. Clicking a tab header switches the active tab. The companion **TabPanel** component defines each tab within the container.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/TabContainer

## Features Supported in Blazor

- `ActiveTabIndex` — Zero-based index of the currently active tab
- `OnActiveTabChanged` — Event raised when the active tab changes
- `OnClientActiveTabChanged` — JavaScript function invoked when the active tab changes
- `ScrollBars` — Specifies which scrollbars to display on the tab content area
- `CssClass` — CSS class applied to the outer TabContainer div
- `ChildContent` — Contains `TabPanel` child components

### TabPanel Parameters

- `HeaderText` — Text displayed in this tab's header
- `HeaderTemplate` — Custom RenderFragment for the tab header (takes precedence over `HeaderText`)
- `ContentTemplate` — The content rendered in the tab panel body when active
- `Enabled` — Whether this tab can be selected (disabled tabs are rendered but not clickable)

## Web Forms Syntax

```html
<ajaxToolkit:TabContainer
    ID="tabs1"
    runat="server"
    ActiveTabIndex="0"
    OnClientActiveTabChanged="onTabChanged"
    ScrollBars="None"
    CssClass="myTabs">
    <ajaxToolkit:TabPanel ID="tab1" runat="server" HeaderText="General">
        <ContentTemplate>
            <p>General settings content.</p>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel ID="tab2" runat="server" HeaderText="Advanced">
        <ContentTemplate>
            <p>Advanced settings content.</p>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel ID="tab3" runat="server" HeaderText="About" Enabled="false">
        <ContentTemplate>
            <p>About information.</p>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
```

## Blazor Migration

```razor
<TabContainer
    ActiveTabIndex="0"
    OnClientActiveTabChanged="onTabChanged"
    ScrollBars="ScrollBars.None"
    CssClass="myTabs">
    <TabPanel HeaderText="General">
        <ContentTemplate>
            <p>General settings content.</p>
        </ContentTemplate>
    </TabPanel>
    <TabPanel HeaderText="Advanced">
        <ContentTemplate>
            <p>Advanced settings content.</p>
        </ContentTemplate>
    </TabPanel>
    <TabPanel HeaderText="About" Enabled="false">
        <ContentTemplate>
            <p>About information.</p>
        </ContentTemplate>
    </TabPanel>
</TabContainer>
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix, remove `runat="server"` and `ID` attributes. Everything else stays the same!

## Properties Reference

### TabContainer

| Property | Type | Default | Description |
|---|---|---|---|
| `ActiveTabIndex` | `int` | `0` | Zero-based index of the currently active tab |
| `OnActiveTabChanged` | `EventCallback<int>` | — | Raised when the active tab changes |
| `OnClientActiveTabChanged` | `string` | `""` | Name of a client-side JavaScript function invoked when the active tab changes |
| `ScrollBars` | `ScrollBars` | `None` | Specifies which scrollbars to display on the tab content area (`None`, `Horizontal`, `Vertical`, `Both`, `Auto`) |
| `CssClass` | `string` | `""` | CSS class applied to the outer TabContainer div |

### TabPanel

| Property | Type | Default | Description |
|---|---|---|---|
| `HeaderText` | `string` | `""` | Text displayed in this tab's header |
| `HeaderTemplate` | `RenderFragment` | `null` | Custom header content (takes precedence over `HeaderText`) |
| `ContentTemplate` | `RenderFragment` | `null` | Content rendered in the tab body when this tab is active |
| `Enabled` | `bool` | `true` | Whether this tab can be selected. Disabled tabs are rendered but not clickable |

## Usage Examples

### Basic Tab Container

```razor
@rendermode InteractiveServer

<TabContainer ActiveTabIndex="0" CssClass="tab-demo">
    <TabPanel HeaderText="Home">
        <ContentTemplate>
            <h3>Welcome</h3>
            <p>This is the home tab content.</p>
        </ContentTemplate>
    </TabPanel>
    <TabPanel HeaderText="Profile">
        <ContentTemplate>
            <h3>User Profile</h3>
            <p>Profile information goes here.</p>
        </ContentTemplate>
    </TabPanel>
    <TabPanel HeaderText="Settings">
        <ContentTemplate>
            <h3>Settings</h3>
            <p>Application settings.</p>
        </ContentTemplate>
    </TabPanel>
</TabContainer>
```

### Tab Container with Custom Header Templates

```razor
@rendermode InteractiveServer

<TabContainer ActiveTabIndex="0">
    <TabPanel>
        <HeaderTemplate>📧 <strong>Inbox</strong> (3)</HeaderTemplate>
        <ContentTemplate>
            <p>You have 3 new messages.</p>
        </ContentTemplate>
    </TabPanel>
    <TabPanel>
        <HeaderTemplate>📤 Sent</HeaderTemplate>
        <ContentTemplate>
            <p>Sent messages archive.</p>
        </ContentTemplate>
    </TabPanel>
</TabContainer>
```

### Tab Container with Disabled Tab

```razor
@rendermode InteractiveServer

<TabContainer ActiveTabIndex="0" OnActiveTabChanged="OnTabChanged">
    <TabPanel HeaderText="Step 1: Info">
        <ContentTemplate>
            <p>Enter your information.</p>
        </ContentTemplate>
    </TabPanel>
    <TabPanel HeaderText="Step 2: Review" Enabled="@step2Enabled">
        <ContentTemplate>
            <p>Review your submission.</p>
        </ContentTemplate>
    </TabPanel>
    <TabPanel HeaderText="Step 3: Confirm" Enabled="false">
        <ContentTemplate>
            <p>Confirm and submit.</p>
        </ContentTemplate>
    </TabPanel>
</TabContainer>

@code {
    private bool step2Enabled = false;

    void OnTabChanged(int index)
    {
        // Enable step 2 after visiting step 1
        step2Enabled = true;
    }
}
```

### Tab Container with JavaScript Callback

```razor
@rendermode InteractiveServer

<TabContainer
    ActiveTabIndex="0"
    OnClientActiveTabChanged="handleTabChange">
    <TabPanel HeaderText="Tab A">
        <ContentTemplate><p>Content A</p></ContentTemplate>
    </TabPanel>
    <TabPanel HeaderText="Tab B">
        <ContentTemplate><p>Content B</p></ContentTemplate>
    </TabPanel>
</TabContainer>

<script>
    function handleTabChange(tabIndex) {
        console.log('Switched to tab:', tabIndex);
    }
</script>
```

## HTML Output

The TabContainer renders a structured layout with a header strip and body area:

```html
<div id="..." class="ajax__tab_container myTabs">
    <!-- Tab header strip -->
    <div class="ajax__tab_header" role="tablist">
        <span class="ajax__tab_active" role="tab" aria-selected="true" style="cursor:pointer;">
            General
        </span>
        <span class="ajax__tab_inactive" role="tab" aria-selected="false" style="cursor:pointer;">
            Advanced
        </span>
        <span class="ajax__tab_inactive ajax__tab_disabled" role="tab" aria-selected="false">
            About
        </span>
    </div>

    <!-- Tab body area -->
    <div class="ajax__tab_body">
        <div class="ajax__tab_panel" role="tabpanel">
            <p>General settings content.</p>
        </div>
        <div class="ajax__tab_panel" role="tabpanel" style="display:none;">
            <p>Advanced settings content.</p>
        </div>
        <div class="ajax__tab_panel" role="tabpanel" style="display:none;">
            <p>About information.</p>
        </div>
    </div>
</div>
```

## JavaScript Interop

The TabContainer loads `tab-container.js` as an ES module only when `OnClientActiveTabChanged` is set. JavaScript handles:

- `invokeClientCallback()` — Calls the named JavaScript function with the new tab index

Tab switching itself is handled entirely by Blazor; JavaScript is only used for the client callback feature.

## Render Mode Requirements

The TabContainer works in all render modes for basic tab switching. **InteractiveServer** is required for:

- `OnActiveTabChanged` Blazor callback
- `OnClientActiveTabChanged` JavaScript callback

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** Tabs render correctly with the active tab visible. Tab switching won't work without interactivity.
- **JavaScript disabled:** Tab switching works via Blazor server events, but `OnClientActiveTabChanged` won't fire.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:TabContainer
   + <TabContainer
   - <ajaxToolkit:TabPanel
   + <TabPanel
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Keep all properties the same** — `HeaderText`, `ContentTemplate`, `Enabled` all work identically

### Before (Web Forms)

```html
<ajaxToolkit:TabContainer ID="tabs" runat="server" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel ID="tab1" runat="server" HeaderText="Tab 1">
        <ContentTemplate><p>Content 1</p></ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel ID="tab2" runat="server" HeaderText="Tab 2">
        <ContentTemplate><p>Content 2</p></ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
```

### After (Blazor)

```razor
<TabContainer ActiveTabIndex="0">
    <TabPanel HeaderText="Tab 1">
        <ContentTemplate><p>Content 1</p></ContentTemplate>
    </TabPanel>
    <TabPanel HeaderText="Tab 2">
        <ContentTemplate><p>Content 2</p></ContentTemplate>
    </TabPanel>
</TabContainer>
```

## Best Practices

1. **Use meaningful tab labels** — Keep headers short and descriptive
2. **Set a sensible default tab** — `ActiveTabIndex="0"` shows the first tab
3. **Disable tabs logically** — Use `Enabled="false"` for wizard-like step progression
4. **Use HeaderTemplate for rich headers** — Add icons or badges when plain text isn't enough
5. **Accessibility** — Tab headers use `role="tab"` and `aria-selected` automatically

## Troubleshooting

| Issue | Solution |
|---|---|
| No tabs visible | Verify TabPanel components are direct children of TabContainer. Check that at least one TabPanel exists. |
| Tab click does nothing | Ensure `@rendermode InteractiveServer` is set. Check that the tab's `Enabled` property is `true`. |
| TabPanel throws exception | TabPanel must be inside a TabContainer. Verify the nesting structure. |
| Client callback not firing | Verify `OnClientActiveTabChanged` names a valid global JavaScript function. Check browser console for errors. |
| Scrollbars not appearing | Set `ScrollBars` to `Vertical`, `Horizontal`, `Both`, or `Auto`. Ensure content overflows the container. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [Accordion](Accordion.md) — Vertically stacked collapsible panes
- [CollapsiblePanelExtender](CollapsiblePanelExtender.md) — Single panel collapse/expand
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
