### DepartmentPortal Custom Controls Migration Patterns

**By:** Cyclops (Component Dev)

**What:** Migrated all 7 DepartmentPortal custom controls to Blazor using BWFC CustomControls base classes. Established migration patterns for the three base class types.

**Key decisions:**

1. **CompositeControl → WebControl with RenderContents**: EmployeeCard originally used `CompositeControl.CreateChildControls()` with Panel/Label/Image child controls. Migrated to flat `RenderContents(HtmlTextWriter)` since BWFC's WebControl doesn't have a child control tree — all rendering is via HtmlTextWriter. This produces identical HTML output.

2. **IPostBackEventHandler removal**: DepartmentBreadcrumb and PollQuestion both implemented `IPostBackEventHandler`. Replaced with `EventCallback<T>` parameters. PostBack JavaScript references removed entirely — Blazor handles interactivity natively.

3. **ITemplate → RenderFragment via TemplatedWebControl**: SectionPanel used `ITemplate` properties with `InstantiateIn()`. Migrated to `RenderFragment` parameters with `RenderTemplate(writer, fragment)` helper from TemplatedWebControl base class.

4. **EventArgs classes**: Created in the `AfterDepartmentPortal.Components.Controls` namespace (co-located with controls) rather than a separate Models folder, since they're tightly coupled to the controls.

5. **HtmlEncode strategy**: Used `System.Net.WebUtility.HtmlEncode` instead of `System.Web.HttpUtility.HtmlEncode` since System.Web is not available in Blazor.

**Why this matters:** These patterns are reusable for any future custom control migration. The three base class types (WebControl, DataBoundWebControl, TemplatedWebControl) cover the vast majority of Web Forms custom control inheritance patterns.
