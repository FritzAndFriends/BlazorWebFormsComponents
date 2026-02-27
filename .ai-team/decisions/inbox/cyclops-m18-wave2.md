### 2026-02-27: MenuItemStyle sub-components must call SetFontsFromAttributes for Font- attributes
**By:** Cyclops
**What:** Added `this.SetFontsFromAttributes(OtherAttributes)` in `MenuItemStyle.OnInitialized()` after `SetPropertiesFromUnknownAttributes()`. This ensures `Font-Bold`, `Font-Italic`, `Font-Size`, etc. attributes declared on style sub-components (like `<StaticMenuItemStyle Font-Bold="true" />`) are properly applied to the `FontInfo` sub-object.
**Why:** The `SetPropertiesFromUnknownAttributes()` method uses reflection to map attribute names to properties, but `Font-Bold` maps to `Font.Bold` (a sub-property), not a direct property. Without the explicit `SetFontsFromAttributes` call, all Font- attributes were silently ignored on menu style sub-components, causing CSS like `font-weight:bold` to never appear in rendered output.

### 2026-02-27: CheckBox must always render id attribute on input element
**By:** Cyclops
**What:** CheckBox.razor's bare (no-text) `<input>` element was missing the `id="@_inputId"` attribute. Added it to match the behavior of the text-present code paths. Web Forms always renders an `id` on CheckBox inputs regardless of whether `Text` is set.
**Why:** Consistency with Web Forms HTML output and with the text-present code paths in the same component. The bare input path renders `class`, `style`, and `title` but was missing `id`, which would break JavaScript targeting and CSS selectors that rely on the control's ID.
