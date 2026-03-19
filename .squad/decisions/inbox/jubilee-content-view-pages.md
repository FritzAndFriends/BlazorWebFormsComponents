# Decision: Standalone sample pages for Content, ContentPlaceHolder, View

**By:** Jubilee (Sample Writer)
**Date:** 2026-03-19

**What:** Created individual standalone sample pages for Content, ContentPlaceHolder, and View components. Previously these shared group pages (Content/ContentPlaceHolder → `/control-samples/masterpage`, View → `/ControlSamples/MultiView`). Each now has a dedicated route and page.

**Routes:**
- Content → `/ControlSamples/Content`
- ContentPlaceHolder → `/ControlSamples/ContentPlaceHolder`
- View → `/ControlSamples/View`

**Why:** Each component needs its own navigable page for the ComponentCatalog sidebar to link directly to focused demos. Shared pages made it impossible to deep-link to a specific component's samples.

**Files changed:**
- `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/Content/Index.razor` (new)
- `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/ContentPlaceHolder/Index.razor` (new)
- `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/View/Index.razor` (new)
- `samples/AfterBlazorServerSide/ComponentCatalog.cs` (routes updated)
