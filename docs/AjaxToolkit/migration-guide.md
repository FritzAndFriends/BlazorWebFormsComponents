# Ajax Control Toolkit Migration Guide

This guide walks you through migrating an ASP.NET Web Forms application that uses the **Ajax Control Toolkit** to Blazor using **BlazorAjaxToolkitComponents** (BWFC). The library provides drop-in Blazor replacements for 14 commonly used Ajax Control Toolkit extenders and containers, preserving the same property names, behavior, and rendered HTML so your existing CSS and JavaScript continue to work.

## Overview

### What is BlazorAjaxToolkitComponents?

BlazorAjaxToolkitComponents is a companion NuGet package to BlazorWebFormsComponents. While the main BWFC package covers standard ASP.NET Web Forms controls (`<asp:Button>`, `<asp:GridView>`, etc.), this package covers the **Ajax Control Toolkit** extenders — the `<ajaxToolkit:*>` controls that added rich client-side behaviors to Web Forms pages.

### Why Does It Exist?

Many Web Forms applications depend on Ajax Control Toolkit extenders for:

- Confirmation dialogs, input masks, and filtered text boxes
- Modal popups, collapsible panels, and hover menus
- Tabs, accordions, calendars, and autocomplete
- Sliders, numeric spinners, and toggle buttons

Without a migration path for these controls, developers would need to completely rewrite their UI behavior. BlazorAjaxToolkitComponents lets you **keep the same markup** (minus the `ajaxToolkit:` prefix) and the same property names, so migration is mechanical rather than creative.

### Supported Controls

| Control | Type | Description |
|---|---|---|
| `ConfirmButtonExtender` | Extender | Confirmation dialog on button click |
| `FilteredTextBoxExtender` | Extender | Character filtering for text input |
| `ModalPopupExtender` | Extender | Modal popup with overlay backdrop |
| `CollapsiblePanelExtender` | Extender | Collapsible/expandable panel |
| `Accordion` / `AccordionPane` | Container | Vertically stacked collapsible panes |
| `TabContainer` / `TabPanel` | Container | Tabbed content panels |
| `CalendarExtender` | Extender | Popup date picker |
| `AutoCompleteExtender` | Extender | Typeahead suggestions |
| `MaskedEditExtender` | Extender | Input mask formatting |
| `NumericUpDownExtender` | Extender | Numeric spinner buttons |
| `SliderExtender` | Extender | Range slider control |
| `ToggleButtonExtender` | Extender | Image-based checkbox toggle |
| `PopupControlExtender` | Extender | Click-triggered popup panel |
| `HoverMenuExtender` | Extender | Hover-triggered popup menu |

---

## Installation

### 1. Add the NuGet Package

```shell
dotnet add package BlazorAjaxToolkitComponents
```

!!! note
    BlazorAjaxToolkitComponents depends on BlazorWebFormsComponents, which will be installed automatically as a transitive dependency.

### 2. Add @using Statements to _Imports.razor

Add these lines to your project's `_Imports.razor` file:

```razor
@using BlazorAjaxToolkitComponents
@using BlazorAjaxToolkitComponents.Enums
```

### 3. Set Render Mode

Ajax Toolkit extender components require **InteractiveServer** render mode because they depend on JavaScript interoperability. Add this to your `App.razor` or the pages that use extenders:

```razor
@rendermode InteractiveServer
```

### 4. JavaScript Interop Setup

No manual `<script>` reference is required. Extender components load their JavaScript modules automatically using ES module imports from the `_content/BlazorAjaxToolkitComponents/js/` static assets path. Blazor's static web asset system handles this transparently.

Ensure your `App.razor` or host page includes the standard Blazor script:

```html
<script src="_framework/blazor.web.js"></script>
```

---

## Migration Steps

### Step 1: Remove ToolkitScriptManager

The `<ajaxToolkit:ToolkitScriptManager>` is not needed in Blazor. Blazor handles script loading and component lifecycle automatically. Simply delete it:

```diff
- <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />
```

### Step 2: Remove the ajaxToolkit: Prefix

All Ajax Control Toolkit controls use the `ajaxToolkit:` XML namespace prefix. In Blazor, components are referenced by their class name directly:

```diff
- <ajaxToolkit:ConfirmButtonExtender
+ <ConfirmButtonExtender
```

For closing tags:

```diff
- </ajaxToolkit:Accordion>
+ </Accordion>
```

### Step 3: Remove runat="server" and Extender ID Attributes

Blazor components don't use `runat="server"`. The extender's own `ID` attribute is also not needed (it was only used for server-side reference in Web Forms):

```diff
- <ajaxToolkit:ConfirmButtonExtender
-     ID="cbe1"
-     runat="server"
+ <ConfirmButtonExtender
      TargetControlID="btnDelete"
      ConfirmText="Are you sure?" />
```

### Step 4: Map Properties (Most Are 1:1)

The vast majority of properties keep the **same name and type**. BWFC was designed for this:

| Web Forms Property | Blazor Property | Notes |
|---|---|---|
| `TargetControlID` | `TargetControlID` | Same — ID of the target control |
| `ConfirmText` | `ConfirmText` | Same |
| `PopupControlID` | `PopupControlID` | Same |
| `BackgroundCssClass` | `BackgroundCssClass` | Same |
| `Minimum` / `Maximum` | `Minimum` / `Maximum` | Same |
| `FilterType` | `FilterType` | Same — uses `FilterType` enum |
| `Mask` | `Mask` | Same |
| `HeaderText` | `HeaderText` | Same |

Enum values are also preserved. For example, `FilterType="Numbers"` in Web Forms becomes `FilterType="FilterType.Numbers"` in Blazor (using the C# enum syntax).

### Step 5: Handle Event Model Differences

Web Forms uses server-side event handlers wired up declaratively. Blazor uses `EventCallback`:

**Web Forms:**
```html
<ajaxToolkit:TabContainer ID="tabs" runat="server"
    OnActiveTabChanged="tabs_ActiveTabChanged">
```

**Blazor:**
```razor
<TabContainer ActiveTabIndex="0"
    OnActiveTabChanged="HandleTabChanged">

@code {
    void HandleTabChanged(int newIndex)
    {
        // Handle tab change
    }
}
```

Key differences:

- **No `OnClick` wiring on extenders** — Extenders attach behavior to their target control. The target control's `OnClick` (or equivalent) still works normally.
- **EventCallback instead of server events** — `Accordion.SelectedIndexChanged` is an `EventCallback<int>`, `TabContainer.OnActiveTabChanged` is an `EventCallback<int>`.
- **Client-side scripts remain strings** — Properties like `OnOkScript`, `OnCancelScript`, and `OnClientActiveTabChanged` accept JavaScript code strings, just like in Web Forms.

### Step 6: Verify InteractiveServer Render Mode

Extender components require JavaScript interop, which is only available in **InteractiveServer** mode. In Static SSR mode, extenders silently skip initialization (no errors thrown, but no behavior is attached).

---

## Per-Control Migration Examples

### ConfirmButtonExtender

Displays a browser confirmation dialog when a button is clicked. If cancelled, the click is suppressed.

=== "Web Forms"

    ```html
    <asp:Button ID="btnDelete" Text="Delete" OnClick="btnDelete_Click" runat="server" />

    <ajaxToolkit:ConfirmButtonExtender
        ID="cbe1"
        runat="server"
        TargetControlID="btnDelete"
        ConfirmText="Are you sure you want to delete this?"
        ConfirmOnFormSubmit="false" />
    ```

=== "Blazor"

    ```razor
    <Button ID="btnDelete" Text="Delete" OnClick="HandleDelete" />

    <ConfirmButtonExtender
        TargetControlID="btnDelete"
        ConfirmText="Are you sure you want to delete this?"
        ConfirmOnFormSubmit="false" />

    @code {
        void HandleDelete() { /* Runs only if user clicks OK */ }
    }
    ```

**Properties:** `TargetControlID`, `ConfirmText`, `ConfirmOnFormSubmit`, `DisplayModalPopupID` (reserved), `BehaviorID`, `Enabled`

---

### FilteredTextBoxExtender

Restricts input in a TextBox to specified character types, filtering keystrokes in real-time.

=== "Web Forms"

    ```html
    <asp:TextBox ID="txtPhone" runat="server" />

    <ajaxToolkit:FilteredTextBoxExtender
        ID="fte1"
        runat="server"
        TargetControlID="txtPhone"
        FilterType="Numbers"
        ValidChars="-+()" />
    ```

=== "Blazor"

    ```razor
    <TextBox ID="txtPhone" />

    <FilteredTextBoxExtender
        TargetControlID="txtPhone"
        FilterType="FilterType.Numbers"
        ValidChars="-+()" />
    ```

**Properties:** `TargetControlID`, `FilterType`, `ValidChars`, `InvalidChars`, `FilterMode`, `FilterInterval`, `BehaviorID`, `Enabled`

!!! tip
    Enum values use C# syntax in Blazor: `FilterType.Numbers`, `FilterType.Custom`, `FilterMode.ValidChars`, etc.

---

### ModalPopupExtender

Displays a target element as a modal popup with an overlay backdrop. Supports OK/Cancel actions, drag, and drop shadow.

=== "Web Forms"

    ```html
    <asp:Button ID="btnOpen" Text="Open" runat="server" />

    <div ID="myModal" style="display:none; background:white; padding:20px;">
        <p>Modal content</p>
        <button ID="btnOK">OK</button>
        <button ID="btnCancel">Cancel</button>
    </div>

    <ajaxToolkit:ModalPopupExtender
        ID="mpe1"
        runat="server"
        TargetControlID="btnOpen"
        PopupControlID="myModal"
        BackgroundCssClass="modalBackground"
        OkControlID="btnOK"
        CancelControlID="btnCancel"
        DropShadow="true"
        Drag="true"
        PopupDragHandleControlID="dragHandle" />
    ```

=== "Blazor"

    ```razor
    <Button ID="btnOpen" Text="Open" />

    <div ID="myModal" style="display:none; background:white; padding:20px;">
        <p>Modal content</p>
        <button ID="btnOK">OK</button>
        <button ID="btnCancel">Cancel</button>
    </div>

    <ModalPopupExtender
        TargetControlID="btnOpen"
        PopupControlID="myModal"
        BackgroundCssClass="modalBackground"
        OkControlID="btnOK"
        CancelControlID="btnCancel"
        DropShadow="true"
        Drag="true"
        PopupDragHandleControlID="dragHandle" />
    ```

**Properties:** `TargetControlID`, `PopupControlID`, `BackgroundCssClass`, `OkControlID`, `CancelControlID`, `OnOkScript`, `OnCancelScript`, `DropShadow`, `Drag`, `PopupDragHandleControlID`, `BehaviorID`, `Enabled`

---

### CollapsiblePanelExtender

Adds collapse/expand behavior to a panel with smooth CSS transitions.

=== "Web Forms"

    ```html
    <asp:Panel ID="pnlContent" runat="server">
        <p>Collapsible content here</p>
    </asp:Panel>

    <asp:Label ID="lblStatus" runat="server" />
    <asp:LinkButton ID="lnkToggle" runat="server" Text="Toggle" />

    <ajaxToolkit:CollapsiblePanelExtender
        ID="cpe1"
        runat="server"
        TargetControlID="pnlContent"
        CollapseControlID="lnkToggle"
        ExpandControlID="lnkToggle"
        Collapsed="true"
        CollapsedText="Show Details"
        ExpandedText="Hide Details"
        TextLabelID="lblStatus"
        ExpandDirection="Vertical"
        CollapsedSize="0" />
    ```

=== "Blazor"

    ```razor
    <Panel ID="pnlContent">
        <p>Collapsible content here</p>
    </Panel>

    <Label ID="lblStatus" />
    <LinkButton ID="lnkToggle" Text="Toggle" />

    <CollapsiblePanelExtender
        TargetControlID="pnlContent"
        CollapseControlID="lnkToggle"
        ExpandControlID="lnkToggle"
        Collapsed="true"
        CollapsedText="Show Details"
        ExpandedText="Hide Details"
        TextLabelID="lblStatus"
        ExpandDirection="ExpandDirection.Vertical"
        CollapsedSize="0" />
    ```

**Properties:** `TargetControlID`, `CollapseControlID`, `ExpandControlID`, `Collapsed`, `CollapsedSize`, `ExpandedSize`, `CollapsedText`, `ExpandedText`, `TextLabelID`, `ExpandDirection`, `AutoCollapse`, `AutoExpand`, `ScrollContents`, `BehaviorID`, `Enabled`

---

### Accordion / AccordionPane

A container that displays collapsible content panes. Only one pane is expanded at a time.

=== "Web Forms"

    ```html
    <ajaxToolkit:Accordion ID="acc1" runat="server"
        SelectedIndex="0"
        HeaderCssClass="accordionHeader"
        ContentCssClass="accordionContent"
        FadeTransitions="true"
        TransitionDuration="300"
        RequireOpenedPane="true"
        SuppressHeaderPostbacks="true">
        <Panes>
            <ajaxToolkit:AccordionPane>
                <Header>Section 1</Header>
                <Content>
                    <p>Content for section 1</p>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane>
                <Header>Section 2</Header>
                <Content>
                    <p>Content for section 2</p>
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>
    </ajaxToolkit:Accordion>
    ```

=== "Blazor"

    ```razor
    <Accordion
        SelectedIndex="0"
        HeaderCssClass="accordionHeader"
        ContentCssClass="accordionContent"
        FadeTransitions="true"
        TransitionDuration="300"
        RequireOpenedPane="true"
        SuppressHeaderPostbacks="true"
        SelectedIndexChanged="HandleSelectionChanged">
        <AccordionPane>
            <Header>Section 1</Header>
            <Content>
                <p>Content for section 1</p>
            </Content>
        </AccordionPane>
        <AccordionPane>
            <Header>Section 2</Header>
            <Content>
                <p>Content for section 2</p>
            </Content>
        </AccordionPane>
    </Accordion>

    @code {
        void HandleSelectionChanged(int index) { /* Handle pane change */ }
    }
    ```

!!! note "Structural Difference"
    In Web Forms, panes are wrapped in a `<Panes>` collection element. In Blazor, `AccordionPane` components are placed directly inside `Accordion` as child content — no `<Panes>` wrapper needed.

**Accordion Properties:** `SelectedIndex`, `SelectedIndexChanged`, `FadeTransitions`, `TransitionDuration`, `HeaderCssClass`, `ContentCssClass`, `RequireOpenedPane`, `AutoSize`, `SuppressHeaderPostbacks`, `CssClass`

**AccordionPane Properties:** `Header` (RenderFragment), `Content` (RenderFragment)

---

### TabContainer / TabPanel

Displays content in tabbed panels with only the active tab visible.

=== "Web Forms"

    ```html
    <ajaxToolkit:TabContainer ID="tabs1" runat="server"
        ActiveTabIndex="0"
        OnClientActiveTabChanged="onTabChanged"
        CssClass="myTabs">
        <ajaxToolkit:TabPanel ID="tab1" runat="server"
            HeaderText="Tab 1">
            <ContentTemplate>
                <p>Content for tab 1</p>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="tab2" runat="server"
            HeaderText="Tab 2">
            <ContentTemplate>
                <p>Content for tab 2</p>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    ```

=== "Blazor"

    ```razor
    <TabContainer
        ActiveTabIndex="0"
        OnClientActiveTabChanged="onTabChanged"
        CssClass="myTabs"
        OnActiveTabChanged="HandleTabChanged">
        <TabPanel HeaderText="Tab 1">
            <ContentTemplate>
                <p>Content for tab 1</p>
            </ContentTemplate>
        </TabPanel>
        <TabPanel HeaderText="Tab 2">
            <ContentTemplate>
                <p>Content for tab 2</p>
            </ContentTemplate>
        </TabPanel>
    </TabContainer>

    @code {
        void HandleTabChanged(int newIndex) { /* Handle tab change */ }
    }
    ```

**TabContainer Properties:** `ActiveTabIndex`, `OnActiveTabChanged`, `OnClientActiveTabChanged`, `ScrollBars`, `CssClass`

**TabPanel Properties:** `HeaderText`, `HeaderTemplate` (RenderFragment), `ContentTemplate` (RenderFragment), `Enabled`

---

### CalendarExtender

Attaches a popup calendar date picker to a TextBox.

=== "Web Forms"

    ```html
    <asp:TextBox ID="txtDate" runat="server" />

    <ajaxToolkit:CalendarExtender
        ID="ce1"
        runat="server"
        TargetControlID="txtDate"
        Format="MM/dd/yyyy"
        PopupPosition="BottomLeft"
        StartDate="2024-01-01"
        EndDate="2025-12-31"
        CssClass="calendarPopup" />
    ```

=== "Blazor"

    ```razor
    <TextBox ID="txtDate" />

    <CalendarExtender
        TargetControlID="txtDate"
        Format="MM/dd/yyyy"
        PopupPosition="CalendarPosition.BottomLeft"
        StartDate="@(new DateTime(2024, 1, 1))"
        EndDate="@(new DateTime(2025, 12, 31))"
        CssClass="calendarPopup" />
    ```

**Properties:** `TargetControlID`, `Format`, `PopupPosition`, `DefaultView`, `StartDate`, `EndDate`, `SelectedDate`, `TodaysDate`, `CssClass`, `OnClientDateSelectionChanged`, `BehaviorID`, `Enabled`

!!! tip
    Date properties (`StartDate`, `EndDate`, `SelectedDate`, `TodaysDate`) are `DateTime?` in Blazor. Use C# expressions like `@(new DateTime(2024, 1, 1))` instead of string literals.

---

### AutoCompleteExtender

Provides typeahead/autocomplete functionality for a TextBox.

=== "Web Forms"

    ```html
    <asp:TextBox ID="txtSearch" runat="server" />

    <ajaxToolkit:AutoCompleteExtender
        ID="ace1"
        runat="server"
        TargetControlID="txtSearch"
        ServicePath="~/AutoCompleteService.asmx"
        ServiceMethod="GetCompletionList"
        MinimumPrefixLength="2"
        CompletionSetCount="10"
        CompletionInterval="500"
        CompletionListCssClass="autocomplete-list"
        CompletionListItemCssClass="autocomplete-item"
        CompletionListHighlightedItemCssClass="autocomplete-highlight"
        FirstRowSelected="true" />
    ```

=== "Blazor"

    ```razor
    <TextBox ID="txtSearch" />

    <AutoCompleteExtender
        TargetControlID="txtSearch"
        ServicePath="/api/autocomplete"
        ServiceMethod="GetCompletionList"
        MinimumPrefixLength="2"
        CompletionSetCount="10"
        CompletionInterval="500"
        CompletionListCssClass="autocomplete-list"
        CompletionListItemCssClass="autocomplete-item"
        CompletionListHighlightedItemCssClass="autocomplete-highlight"
        FirstRowSelected="true"
        CompletionDataRequested="HandleCompletionRequest" />

    @code {
        async Task HandleCompletionRequest(string prefix)
        {
            // Fetch suggestions server-side
        }
    }
    ```

**Properties:** `TargetControlID`, `ServicePath`, `ServiceMethod`, `MinimumPrefixLength`, `CompletionSetCount`, `CompletionInterval`, `CompletionListCssClass`, `CompletionListItemCssClass`, `CompletionListHighlightedItemCssClass`, `DelimiterCharacters`, `FirstRowSelected`, `ShowOnlyCurrentWordInCompletionListItem`, `OnClientItemSelected`, `OnClientPopulating`, `OnClientPopulated`, `CompletionDataRequested`, `BehaviorID`, `Enabled`

!!! note
    The Blazor version adds `CompletionDataRequested` — an `EventCallback<string>` for fetching suggestions server-side, replacing the Web Forms ASMX service pattern.

---

### MaskedEditExtender

Applies an input mask to a TextBox, formatting user input according to a pattern.

=== "Web Forms"

    ```html
    <asp:TextBox ID="txtPhone" runat="server" />

    <ajaxToolkit:MaskedEditExtender
        ID="mee1"
        runat="server"
        TargetControlID="txtPhone"
        Mask="(999) 999-9999"
        MaskType="Number"
        InputDirection="LeftToRight"
        ClearMaskOnLostFocus="true"
        PromptCharacter="_" />
    ```

=== "Blazor"

    ```razor
    <TextBox ID="txtPhone" />

    <MaskedEditExtender
        TargetControlID="txtPhone"
        Mask="(999) 999-9999"
        MaskType="MaskType.Number"
        InputDirection="InputDirection.LeftToRight"
        ClearMaskOnLostFocus="true"
        PromptCharacter="_" />
    ```

**Properties:** `TargetControlID`, `Mask`, `MaskType`, `InputDirection`, `PromptCharacter`, `AutoComplete`, `AutoCompleteValue`, `Filtered`, `ClearMaskOnLostFocus`, `ClearTextOnInvalid`, `AcceptAMPM`, `AcceptNegative`, `ErrorTooltipEnabled`, `ErrorTooltipCssClass`, `BehaviorID`, `Enabled`

---

### NumericUpDownExtender

Adds numeric up/down spinner buttons to a TextBox.

=== "Web Forms"

    ```html
    <asp:TextBox ID="txtQuantity" runat="server" Text="1" />

    <ajaxToolkit:NumericUpDownExtender
        ID="nud1"
        runat="server"
        TargetControlID="txtQuantity"
        Width="120"
        Minimum="1"
        Maximum="100"
        Step="1" />
    ```

=== "Blazor"

    ```razor
    <TextBox ID="txtQuantity" Text="1" />

    <NumericUpDownExtender
        TargetControlID="txtQuantity"
        Width="120"
        Minimum="1"
        Maximum="100"
        Step="1" />
    ```

**Properties:** `TargetControlID`, `Width`, `Minimum`, `Maximum`, `Step`, `RefValues`, `ServiceUpPath`, `ServiceUpMethod`, `ServiceDownPath`, `ServiceDownMethod`, `Tag`, `BehaviorID`, `Enabled`

---

### SliderExtender

Attaches range slider behavior to a target input element.

=== "Web Forms"

    ```html
    <asp:TextBox ID="txtSlider" runat="server" />
    <asp:Label ID="lblValue" runat="server" />

    <ajaxToolkit:SliderExtender
        ID="se1"
        runat="server"
        TargetControlID="txtSlider"
        BoundControlID="lblValue"
        Minimum="0"
        Maximum="100"
        Steps="10"
        Orientation="Horizontal"
        RailCssClass="sliderRail"
        HandleCssClass="sliderHandle"
        TooltipText="Value: {0}" />
    ```

=== "Blazor"

    ```razor
    <TextBox ID="txtSlider" />
    <Label ID="lblValue" />

    <SliderExtender
        TargetControlID="txtSlider"
        BoundControlID="lblValue"
        Minimum="0"
        Maximum="100"
        Steps="10"
        Orientation="SliderOrientation.Horizontal"
        RailCssClass="sliderRail"
        HandleCssClass="sliderHandle"
        TooltipText="Value: {0}" />
    ```

**Properties:** `TargetControlID`, `Minimum`, `Maximum`, `Steps`, `Value`, `BoundControlID`, `Orientation`, `RailCssClass`, `HandleCssClass`, `HandleImageUrl`, `Length`, `Decimals`, `TooltipText`, `BehaviorID`, `Enabled`

---

### ToggleButtonExtender

Replaces a checkbox with a clickable image that toggles between checked and unchecked states.

=== "Web Forms"

    ```html
    <asp:CheckBox ID="chkAgree" runat="server" />

    <ajaxToolkit:ToggleButtonExtender
        ID="tbe1"
        runat="server"
        TargetControlID="chkAgree"
        ImageWidth="24"
        ImageHeight="24"
        UncheckedImageUrl="~/images/unchecked.png"
        CheckedImageUrl="~/images/checked.png"
        UncheckedImageAlternateText="Not agreed"
        CheckedImageAlternateText="Agreed"
        CheckedImageOverUrl="~/images/checked-hover.png"
        UncheckedImageOverUrl="~/images/unchecked-hover.png" />
    ```

=== "Blazor"

    ```razor
    <CheckBox ID="chkAgree" />

    <ToggleButtonExtender
        TargetControlID="chkAgree"
        ImageWidth="24"
        ImageHeight="24"
        UncheckedImageUrl="images/unchecked.png"
        CheckedImageUrl="images/checked.png"
        UncheckedImageAlternateText="Not agreed"
        CheckedImageAlternateText="Agreed"
        CheckedImageOverUrl="images/checked-hover.png"
        UncheckedImageOverUrl="images/unchecked-hover.png" />
    ```

**Properties:** `TargetControlID`, `ImageWidth`, `ImageHeight`, `UncheckedImageUrl`, `CheckedImageUrl`, `UncheckedImageAlternateText`, `CheckedImageAlternateText`, `CheckedImageOverUrl`, `UncheckedImageOverUrl`, `DisabledUncheckedImageUrl`, `DisabledCheckedImageUrl`, `BehaviorID`, `Enabled`

!!! tip
    Replace `~/` paths with relative paths in Blazor. Static assets go in `wwwroot/`.

---

### PopupControlExtender

Attaches a popup panel to a target control, displaying it on click. Lighter than ModalPopupExtender — no overlay or focus trap.

=== "Web Forms"

    ```html
    <asp:TextBox ID="txtColor" runat="server" />

    <asp:Panel ID="pnlColorPicker" runat="server" style="display:none;">
        <!-- Color picker content -->
    </asp:Panel>

    <ajaxToolkit:PopupControlExtender
        ID="pce1"
        runat="server"
        TargetControlID="txtColor"
        PopupControlID="pnlColorPicker"
        Position="Bottom"
        OffsetX="0"
        OffsetY="2"
        CommitProperty="value"
        CommitScript="onColorSelected()" />
    ```

=== "Blazor"

    ```razor
    <TextBox ID="txtColor" />

    <Panel ID="pnlColorPicker" style="display:none;">
        @* Color picker content *@
    </Panel>

    <PopupControlExtender
        TargetControlID="txtColor"
        PopupControlID="pnlColorPicker"
        Position="PopupPosition.Bottom"
        OffsetX="0"
        OffsetY="2"
        CommitProperty="value"
        CommitScript="onColorSelected()" />
    ```

**Properties:** `TargetControlID`, `PopupControlID`, `Position`, `OffsetX`, `OffsetY`, `CommitProperty`, `CommitScript`, `ExtenderControlID`, `BehaviorID`, `Enabled`

---

### HoverMenuExtender

Displays a popup panel when the user hovers over a target control.

=== "Web Forms"

    ```html
    <asp:Label ID="lblItem" runat="server" Text="Hover over me" />

    <asp:Panel ID="pnlMenu" runat="server" CssClass="hoverMenu" style="display:none;">
        <a href="#">Edit</a>
        <a href="#">Delete</a>
    </asp:Panel>

    <ajaxToolkit:HoverMenuExtender
        ID="hme1"
        runat="server"
        TargetControlID="lblItem"
        PopupControlID="pnlMenu"
        PopupPosition="Right"
        OffsetX="5"
        OffsetY="0"
        PopDelay="200"
        HoverDelay="500"
        HoverCssClass="hoverHighlight" />
    ```

=== "Blazor"

    ```razor
    <Label ID="lblItem" Text="Hover over me" />

    <Panel ID="pnlMenu" CssClass="hoverMenu" style="display:none;">
        <a href="#">Edit</a>
        <a href="#">Delete</a>
    </Panel>

    <HoverMenuExtender
        TargetControlID="lblItem"
        PopupControlID="pnlMenu"
        PopupPosition="PopupPosition.Right"
        OffsetX="5"
        OffsetY="0"
        PopDelay="200"
        HoverDelay="500"
        HoverCssClass="hoverHighlight" />
    ```

**Properties:** `TargetControlID`, `PopupControlID`, `PopupPosition`, `OffsetX`, `OffsetY`, `PopDelay`, `HoverDelay`, `HoverCssClass`, `BehaviorID`, `Enabled`

---

## L1 Script Support

The **bwfc-migrate.ps1** script (located in `migration-toolkit/scripts/`) automates much of the Ajax Control Toolkit conversion as part of the L1 (Level 1) mechanical migration pass.

### What the Script Handles Automatically

1. **Removes `<ajaxToolkit:ToolkitScriptManager>`** — Both self-closing and block forms are stripped entirely since Blazor doesn't need them.

2. **Converts known `ajaxToolkit:` prefixes** — For all 16 supported components (the 14 controls listed above plus their companion types), the script strips the `ajaxToolkit:` prefix from both opening and closing tags:
    - `<ajaxToolkit:Accordion>` → `<Accordion>`
    - `</ajaxToolkit:AccordionPane>` → `</AccordionPane>`

3. **Flags unrecognized controls** — Any `ajaxToolkit:` control not in the known list is replaced with a `TODO` comment:
    ```razor
    @* TODO: Convert ajaxToolkit:RatingExtender — no BWFC equivalent yet *@
    ```

4. **Adds `@using` statements** — When Ajax Toolkit controls are detected, the script ensures `@using BlazorAjaxToolkitComponents` is added to `_Imports.razor`.

### Running the Script

```powershell
.\migration-toolkit\scripts\bwfc-migrate.ps1 -SourcePath "C:\MyWebFormsApp" -OutputPath "C:\MyBlazorApp"
```

### What Requires Manual Follow-up (L2)

After the L1 script runs, you may need to:

- Convert enum string values to C# enum syntax (e.g., `FilterType="Numbers"` → `FilterType="FilterType.Numbers"`)
- Replace `~/` asset paths with relative Blazor paths
- Convert server-side event handlers to `EventCallback` syntax
- Remove the `<Panes>` wrapper from Accordion content
- Verify `TargetControlID` references match the migrated control IDs
- Set `@rendermode InteractiveServer` on pages that use extenders

---

## Common Issues & Troubleshooting

### Extender not activating

| Symptom | Cause | Solution |
|---|---|---|
| No behavior on button click | Missing render mode | Add `@rendermode InteractiveServer` to the page |
| Extender initializes but no effect | Wrong `TargetControlID` | Verify the ID matches the target control's `ID` attribute exactly (case-sensitive) |
| JS console error on import | Missing static assets | Ensure `BlazorAjaxToolkitComponents` NuGet package is installed and project is built |
| Works in dev, fails in production | Static asset path issue | Verify `_content/BlazorAjaxToolkitComponents/` is being served |

### Enum values not recognized

Blazor requires C# enum syntax. String values from Web Forms markup need conversion:

```diff
- FilterType="Numbers"
+ FilterType="FilterType.Numbers"

- MaskType="Number"
+ MaskType="MaskType.Number"

- Orientation="Horizontal"
+ Orientation="SliderOrientation.Horizontal"
```

### TargetControlID not found at runtime

- Ensure the target control renders **before** the extender in the component tree
- JavaScript resolves the target via `document.getElementById()` — the element must exist in the DOM
- If the target is inside a conditional `@if` block, the extender may initialize before the target renders

### Accordion panes not rendering

- Remove the `<Panes>` wrapper — Blazor uses direct child content
- Ensure each `AccordionPane` is a direct child of `Accordion`
- Check that `Header` and `Content` render fragments are present

### Modal not closing

- Verify `OkControlID` and `CancelControlID` match the button `ID` attributes inside the modal
- Ensure buttons are inside the `PopupControlID` element
- Check that the popup element has `style="display:none;"` initially

### Calendar dates as strings

Date properties are `DateTime?` in Blazor, not strings:

```diff
- StartDate="2024-01-01"
+ StartDate="@(new DateTime(2024, 1, 1))"
```

### Static SSR mode — extenders don't work

Extenders require JavaScript interop which is only available in InteractiveServer mode. In Static SSR, extenders silently skip initialization. Wrap extender usage in an interactive component:

```razor
@rendermode InteractiveServer

@* All extenders on this page will work *@
```

---

## Feature Comparison Table

The table below shows which Ajax Control Toolkit features are supported in BlazorAjaxToolkitComponents.

### Legend

| Status | Meaning |
|---|---|
| ✅ Supported | Fully implemented with same behavior |
| ⚠️ Partial | Implemented with limitations — see notes |
| 🔮 Planned | Not yet implemented — reserved for future versions |
| ❌ Not Planned | Will not be implemented |

### Control Support Matrix

| Ajax Control Toolkit Control | BWFC Status | Notes |
|---|---|---|
| **ConfirmButtonExtender** | ✅ Supported | `DisplayModalPopupID` reserved for future use |
| **FilteredTextBoxExtender** | ✅ Supported | All filter types and modes supported |
| **ModalPopupExtender** | ✅ Supported | Full support including drag, drop shadow, focus trap |
| **CollapsiblePanelExtender** | ✅ Supported | Vertical and horizontal expand directions |
| **Accordion** | ✅ Supported | Includes JS animation support |
| **AccordionPane** | ✅ Supported | Direct child content (no `<Panes>` wrapper) |
| **TabContainer** | ✅ Supported | Client callback, scrollbar support |
| **TabPanel** | ✅ Supported | HeaderText, HeaderTemplate, and Enabled |
| **CalendarExtender** | ✅ Supported | Date ranges, views, and format strings |
| **AutoCompleteExtender** | ✅ Supported | Adds `CompletionDataRequested` EventCallback |
| **MaskedEditExtender** | ✅ Supported | All mask types and input directions |
| **NumericUpDownExtender** | ✅ Supported | Step, range, and reference values |
| **SliderExtender** | ✅ Supported | Horizontal/vertical, bound control sync |
| **ToggleButtonExtender** | ✅ Supported | All image states including disabled |
| **PopupControlExtender** | ✅ Supported | Positional placement and commit support |
| **HoverMenuExtender** | ✅ Supported | Show/hide delays and hover CSS class |
| ToolkitScriptManager | ❌ Not Planned | Not needed — Blazor handles script loading |
| AlwaysVisibleControlExtender | 🔮 Planned | — |
| AnimationExtender | 🔮 Planned | — |
| CascadingDropDown | 🔮 Planned | — |
| ColorPickerExtender | 🔮 Planned | — |
| DragPanelExtender | 🔮 Planned | — |
| DropDownExtender | 🔮 Planned | — |
| DropShadowExtender | 🔮 Planned | — |
| DynamicPopulateExtender | 🔮 Planned | — |
| ListSearchExtender | 🔮 Planned | — |
| MutuallyExclusiveCheckBoxExtender | 🔮 Planned | — |
| PagingBulletedListExtender | 🔮 Planned | — |
| PasswordStrength | 🔮 Planned | — |
| Rating | 🔮 Planned | — |
| ReorderList | 🔮 Planned | — |
| ResizableControlExtender | 🔮 Planned | — |
| RoundedCornersExtender | 🔮 Planned | — |
| TextBoxWatermarkExtender | 🔮 Planned | — |
| UpdatePanelAnimationExtender | 🔮 Planned | — |
| ValidatorCalloutExtender | 🔮 Planned | — |

---

## See Also

- [Ajax Control Toolkit Overview](index.md) — Component index, usage patterns, and common properties
- [Accordion](Accordion.md) — Collapsible content panes
- [AutoCompleteExtender](AutoCompleteExtender.md) — Typeahead suggestions
- [CalendarExtender](CalendarExtender.md) — Popup date picker
- [CollapsiblePanelExtender](CollapsiblePanelExtender.md) — Panel collapse/expand
- [ConfirmButtonExtender](ConfirmButtonExtender.md) — Browser confirmation dialogs
- [FilteredTextBoxExtender](FilteredTextBoxExtender.md) — Character filtering
- [HoverMenuExtender](HoverMenuExtender.md) — Hover-triggered popup
- [MaskedEditExtender](MaskedEditExtender.md) — Input mask formatting
- [ModalPopupExtender](ModalPopupExtender.md) — Modal popup dialogs
- [NumericUpDownExtender](NumericUpDownExtender.md) — Numeric spinner
- [PopupControlExtender](PopupControlExtender.md) — Click-triggered popup
- [SliderExtender](SliderExtender.md) — Range slider
- [TabContainer](TabContainer.md) — Tabbed content panels
- [ToggleButtonExtender](ToggleButtonExtender.md) — Image toggle for checkboxes
- [Automated Migration Guide](../Migration/AutomatedMigration.md) — Full bwfc-migrate.ps1 documentation
