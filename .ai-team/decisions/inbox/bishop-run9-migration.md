### 2025-07-25: Layer 1 script bugs — ItemType conversion and validator type params

**By:** Bishop
**What:** `bwfc-migrate.ps1` has three bugs that cause build failures in every migration run:
1. Converts `ItemType` to `TItem` for ALL data controls, but GridView/ListView/FormView/DetailsView use `ItemType` as their type parameter name (only DropDownList uses `TItem`).
2. Does not add `Type="string"` to RequiredFieldValidator/RegularExpressionValidator or `InputType="string"` to CompareValidator.
3. Does not add `@using BlazorWebFormsComponents.Validations` to generated `_Imports.razor`.
**Why:** These three issues require manual Layer 2 fixes in every migration run. Fixing them in bwfc-migrate.ps1 would eliminate ~30 minutes of build-fix iteration. Discovered during Run 9 benchmark (7 build attempts needed).

### 2025-07-25: @inherits WebFormsPageBase conflicts with `: ComponentBase`

**By:** Bishop
**What:** When `_Imports.razor` contains `@inherits WebFormsPageBase`, all code-behind files must NOT specify `: ComponentBase` as their base class (causes CS0263). Exception: Layout files must specify `: LayoutComponentBase` in both the .razor file (`@inherits LayoutComponentBase`) and code-behind.
**Why:** The Layer 1 script generates code-behinds with `: ComponentBase` which conflicts with the global `@inherits`. This causes ~30 CS0263 errors per run. The fix pattern should be documented and automated.
