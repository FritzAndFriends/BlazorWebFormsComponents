# MultiView / View

The **MultiView** and **View** components emulate the ASP.NET Web Forms `asp:MultiView` and `asp:View` controls. MultiView is a container that holds multiple View controls, displaying only one at a time based on the `ActiveViewIndex` property.

Original Microsoft documentation:

- MultiView: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.multiview?view=netframework-4.8
- View: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.view?view=netframework-4.8

## Blazor Features Supported

### MultiView
- `ActiveViewIndex` — index of the currently visible View (-1 = none)
- `OnActiveViewChanged` — event fired when the active view changes
- `GetActiveView()` — returns the currently active View
- `SetActiveView(View)` — sets the active view by reference
- Command name constants: `NextViewCommandName`, `PreviousViewCommandName`, `SwitchViewByIDCommandName`, `SwitchViewByIndexCommandName`
- Child `View` components auto-register via `CascadingParameter`

### View
- `ChildContent` — arbitrary child content rendered when active
- `OnActivate` — fired when this View becomes active
- `OnDeactivate` — fired when this View is deactivated
- `Visible` — controlled by parent MultiView

## Web Forms Features NOT Supported

- Command-based navigation via `BubbleEvent` (use `ActiveViewIndex` binding instead)
- `EnableTheming` / `SkinID` — theming not supported in Blazor

## Syntax Comparison

=== "Web Forms (Before)"

    ```html
    <asp:MultiView ID="MultiView1" ActiveViewIndex="0" runat="server">
        <asp:View ID="View1" runat="server">
            <p>This is View 1</p>
        </asp:View>
        <asp:View ID="View2" runat="server">
            <p>This is View 2</p>
        </asp:View>
    </asp:MultiView>
    ```

=== "Blazor (After)"

    ```razor
    <MultiView ActiveViewIndex="@activeIndex" OnActiveViewChanged="ViewChanged">
        <View>
            <p>This is View 1</p>
            <Button Text="Next" OnClick="() => activeIndex = 1" />
        </View>
        <View>
            <p>This is View 2</p>
            <Button Text="Previous" OnClick="() => activeIndex = 0" />
        </View>
    </MultiView>

    @code {
        private int activeIndex = 0;

        private void ViewChanged(EventArgs e)
        {
            // Handle view change
        }
    }
    ```

## HTML Output

MultiView and View render **no wrapper HTML elements**. Only the active View's child content is rendered directly into the DOM.

**Blazor Input:**
```razor
<MultiView ActiveViewIndex="0">
    <View><p>Hello</p></View>
    <View><p>World</p></View>
</MultiView>
```

**Rendered HTML:**
```html
<p>Hello</p>
```

!!! tip "Migration Notes"
    1. **Remove `asp:` prefix** — Change `<asp:MultiView>` to `<MultiView>` and `<asp:View>` to `<View>`
    2. **Remove `runat="server"`** — Not needed in Blazor
    3. **Bind ActiveViewIndex** — Use `@activeIndex` binding instead of code-behind manipulation
    4. **Replace command navigation** — Web Forms uses `NextView`/`PrevView` command names with Button controls. In Blazor, bind button clicks to change `ActiveViewIndex` directly.

## Example Migration

=== "Web Forms (Before)"

    ```html
    <asp:MultiView ID="MultiView1" ActiveViewIndex="0" runat="server">
        <asp:View ID="View1" runat="server">
            <asp:Button ID="btn1" CommandName="NextView" Text="Next" runat="server" />
        </asp:View>
        <asp:View ID="View2" runat="server">
            <asp:Button ID="btn2" CommandName="PrevView" Text="Previous" runat="server" />
        </asp:View>
    </asp:MultiView>
    ```

=== "Blazor (After)"

    ```razor
    <MultiView ActiveViewIndex="@activeIndex">
        <View>
            <Button Text="Next" OnClick="() => activeIndex = 1" />
        </View>
        <View>
            <Button Text="Previous" OnClick="() => activeIndex = 0" />
        </View>
    </MultiView>

    @code {
        private int activeIndex = 0;
    }
    ```

!!! note "Key Difference"
    In Web Forms, view navigation used command names like `NextView` and `PrevView` bubbled through Button controls. In Blazor, you directly bind button click events to change the `ActiveViewIndex` property.

## See Also

- [Panel](../EditorControls/Panel.md) — Another container component
