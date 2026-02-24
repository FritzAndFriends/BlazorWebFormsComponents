### PagerSettings follows settings-not-style pattern

**By:** Cyclops
**What:** PagerSettings is a plain C# class (not inheriting `Style`), unlike the existing `TableItemStyle` sub-components. The `UiPagerSettings` base component extends `ComponentBase` directly (not `UiStyle<T>`) because PagerSettings has no visual style properties — it's pure configuration. The same CascadingParameter pattern is used (`IPagerSettingsContainer` interface, cascaded `"ParentXxx"` value), but the base class is simpler. The `PagerButtons` enum already existed; only `PagerPosition` was new.
**Why:** Future sub-components that configure behavior (not style) should follow this `UiPagerSettings` pattern rather than `UiTableItemStyle`. The distinction is: style sub-components inherit `UiStyle<T>` and set visual properties; settings sub-components inherit `ComponentBase` and set configuration properties.
