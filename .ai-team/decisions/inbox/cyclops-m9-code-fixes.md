# Cyclops M9 Code Fixes — Decisions

### ToolTip belongs on BaseStyledComponent

**By:** Cyclops
**What:** `[Parameter] public string ToolTip { get; set; }` is defined on `BaseStyledComponent`, not on individual controls. Removed 8 duplicate declarations (Button, Calendar, DataList, FileUpload, HyperLink, Image, ImageButton, ImageMap).
**Why:** Web Forms `WebControl.ToolTip` is defined at the base class level. All styled controls should inherit it. Sub-component types (ChartSeries, DataPoint, MenuItem, TreeNode) keep their own ToolTip because those are item-level tooltips with different semantics.

### ValidationSummary must not use Split for message extraction

**By:** Cyclops
**What:** `AspNetValidationSummary.ValidationMessages` uses `IndexOf(',')` + `Substring()` instead of `Split(',')[1]` to extract error messages from the field-prefixed format.
**Why:** Error messages may contain commas. `Split(',')[1]` silently truncates the message at the first comma within the message text itself. The field identifier is always before the first comma, so `IndexOf` + `Substring` correctly extracts the full message.

### SkinID is a string, not a bool

**By:** Cyclops
**What:** `BaseWebFormsComponent.SkinID` type changed from `bool` to `string`. The `[Obsolete]` attribute is preserved.
**Why:** Web Forms `Control.SkinID` is a string containing the name of the skin to apply. A boolean makes no sense for this property and would break any migration code that sets `SkinID="MySkin"`.
