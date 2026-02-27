# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

### Core Context (2026-02-10 through 2026-02-27)

**Sample conventions:** Pages in `Components/Pages/ControlSamples/{Name}/Index.razor` (newer .NET 8+ path). Legacy pages in `Pages/ControlSamples/`. Nav updates: NavMenu.razor + ComponentList.razor. `@using BlazorWebFormsComponents.LoginControls` required for login controls. `#pragma warning disable CS0618` for Obsolete APIs.

**M1M4 samples:** Calendar, FileUpload (@ref), ImageMap (List<HotSpot>), PasswordRecovery (3-step), DetailsView (Items parameter). Chart: 8 basic + 4 advanced. DataBinder Eval() demos. ViewState @ref counter.

**M6 samples:** Button AccessKey+ToolTip, GridView CssClass, Validator Display. DropDownList DataTextFormatString, Menu Orientation (Horizontal  requires local variable for enum collision), Label AssociatedControlID.

**M9 Navigation Audit:** ComponentCatalog.cs drives sidebar. Found 4 missing components + 15 missing SubPages. SubPage names must match @page route segments (not file names). DataList "Flow" vs "SimpleFlow" name mismatch.

**M10 catalog fixes:** Added 4 missing components (Menu, DataBinder, PasswordRecovery, ViewState), DetailsView as new entry, 15 SubPages. Follow-up: added 5 more (CheckBoxList, DataPager, ImageButton, ListBox, LoginView). DataList SubPage fix: "SimpleFlow""Flow" (route-based). Build verified.

**M12 BeforeWebForms samples:** DetailsView (2 instances, AutoGenerateRows+explicit BoundFields), DataPager (ListView+DataPager combo). Existing data control samples verified (GridView, DataList, Repeater, FormView, ListView all have audit markers + SharedSampleObjects data).

**SharedSampleObjects alignment:** Created Employee.cs model. Added Product.GetProducts(int count). Aligned GridView (5 files), ListView (1 file) to shared models. Intentionally excluded BulletedList, DropDownList, Chart (different data shapes).

**M15 sample data alignment (#381):** 14 pages aligned to WebForms counterpart text/values/URLs. Label, Literal, HiddenField, PlaceHolder, Panel, HyperLink, Image, Button, CheckBox, DropDownList, BulletedList, LinkButton, ImageMap, AdRotator. Created wwwroot/Content/Images/banner.png. Limitation: Label HTML content  Blazor HTML-encodes @Text.

**M15 audit markers (#384):** 10 pages updated with data-audit-control wrappers. 2 new pages (DataPager, LoginView). Validator samples only first variant marked.

**Key patterns:** ComponentCatalog.cs entries: (Name, Category, Route, Description, SubPages?, Keywords?). SubPages appended to base Route for nav. Components without Index.razor use specific sub-page route. Entries grouped by category, alphabetical within. SharedSampleObjects is single source for data parity. data-audit-control markers must be preserved on all audited sections.

 Team update (2026-02-27): Branching workflow directive  feature PRs from personal fork to upstream dev, only devmain on upstream  decided by Jeffrey T. Fritz

 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax, no manual closures  decided by Jeffrey T. Fritz


 Team update (2026-02-27): AJAX Controls nav category created; migration stub doc pattern for no-op components; Substitution moved from deferred to implemented; UpdateProgress uses explicit state pattern  decided by Beast


 Team update (2026-02-27): M17 AJAX controls implemented  ScriptManager/Proxy are no-op stubs, Timer shadows Enabled, UpdatePanel uses ChildContent, UpdateProgress renders hidden, Substitution uses Func callback, new AJAX/Migration Helper categories  decided by Cyclops
