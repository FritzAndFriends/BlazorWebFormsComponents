# ContosoUniversity Migration - Run 01 Findings

**Date**: 2025-01-20  
**Source Application**: ContosoUniversity (ASP.NET Web Forms)  
**Target Framework**: .NET 10 Blazor Server (SSR)  
**Migration Toolkit**: BWFC Layer 1 + Layer 2 Scripts

## Summary

| Metric | Value |
|--------|-------|
| Layer 1 Transforms | 72 |
| Layer 2 Transforms | 7 (6 code-behinds + 1 Program.cs) |
| Initial Build Errors | 18 |
| Final Build Errors | 0 |
| Final Warnings | 3 |
| Manual Fixes Required | ~25 edits |
| **Acceptance Tests** | **31/40 (77.5%)** |

## Build Error Categories

### 1. Layer 2 Script Bug: Invalid Code Generation

**Error**: `CS1585: Member modifier 'private' must precede the member type and name`

**Example Generated Code**:
```csharp
[Parameter]
public or private { get; set; }
```

**Cause**: The Layer 2 script has a regex bug that generates invalid property syntax when attempting to convert Web Forms properties.

**Fix Applied**: Manually rewrote all 6 code-behind files with proper ComponentBase patterns.

**Automation Opportunity**: **HIGH PRIORITY** - Fix the regex pattern in `bwfc-migrate-layer2.ps1` that generates `public or private { get; set; }`. This affects all migrations.

---

### 2. EF6 DbContext → EF Core Migration

**Error**: `CS0246: The type or namespace name 'DbModelBuilder' could not be found`

**Cause**: The Layer 1 script copies the EF6 DbContext but the EF Core `OnModelCreating` method signature is different.

**Fix Applied**: 
- Changed base class method signature from `OnModelCreating(DbModelBuilder modelBuilder)` to `OnModelCreating(ModelBuilder modelBuilder)`
- Removed `UnintentionalCodeFirstException` throw
- Added proper entity key configuration
- Added relationship mappings

**Automation Opportunity**: **HIGH** - Layer 2 could detect EF6 DbContext patterns and generate EF Core equivalents:
- Convert `DbModelBuilder` to `ModelBuilder`
- Generate key configurations from existing `[Key]` attributes
- Generate relationship mappings from navigation properties

---

### 3. GridView/DetailsView Inline Style Elements Not Supported

**Error**: `RZ9996: Unrecognized child content inside component 'GridView'`

**Cause**: Web Forms uses inline style sub-elements like:
```xml
<AlternatingRowStyle BackColor="White" />
<HeaderStyle BackColor="#507CD1" Font-Bold="True" />
<RowStyle BackColor="#EFF3FB" />
```

BWFC requires wrapper elements:
```xml
<RowStyleContent>
    <GridViewRowStyle BackColor="White" />
</RowStyleContent>
```

**Fix Applied**: Removed inline style elements for simplicity. Styling delegated to CSS.

**Automation Opportunity**: **MEDIUM** - Layer 1 could transform inline style elements to `*Content` wrapper syntax:
- `<RowStyle ...>` → `<RowStyleContent><GridViewRowStyle ... /></RowStyleContent>`

---

### 4. Color Values with Hash (#) Character

**Error**: `CS1024: Preprocessor directive expected`

**Cause**: Blazor Razor parser interprets `#` as a preprocessor directive:
```razor
ForeColor="#333333"  <!-- Interpreted as preprocessor -->
```

**Fix Applied**: Removed color attributes or used named colors (`Black`, `White`, `Blue`).

**Automation Opportunity**: **HIGH** - Layer 1 should:
- Quote hex colors: `ForeColor="@("#333333")"`
- Or convert to named colors where possible
- Or wrap in `WebColor` type: `ForeColor="WebColor.FromHex("#333333")"`

---

### 5. Boolean Attribute Values Case Sensitivity

**Error**: `CS0103: The name 'True' does not exist in the current context`

**Cause**: Web Forms uses PascalCase booleans (`True`/`False`), C# requires lowercase (`true`/`false`).

**Fix Applied**: Changed all `True` to `true` and `False` to `false`.

**Automation Opportunity**: **HIGH** - Layer 1 should normalize:
- `="True"` → `="true"`
- `="False"` → `="false"`

---

### 6. BorderWidth with Unit Suffix

**Error**: `CS1003: Syntax error, ',' expected`

**Cause**: `BorderWidth="2px"` isn't valid for BWFC components expecting `Unit` type.

**Fix Applied**: Removed `BorderWidth` attribute.

**Automation Opportunity**: **LOW** - Requires understanding BWFC Unit API. Could add TODO comments.

---

### 7. ItemType Required on Generic Components

**Warning/Runtime Issue**: Components render incorrectly without type parameters.

**Cause**: GridView, DetailsView, BoundField need `ItemType` specified:
```razor
<GridView ItemType="Student" Items="@_students">
```

**Fix Applied**: Added `ItemType` to all data-bound components.

**Automation Opportunity**: **MEDIUM** - Layer 2 could infer ItemType from:
- The `_data` field type in code-behind
- The entity referenced in SelectMethod queries

---

### 8. Legacy Business Logic Classes

**Error**: `CS7036: There is no argument given that corresponds to the required parameter 'options'`

**Cause**: `Enrollmet_Logic.cs` instantiated DbContext directly: `new ContosoUniversityEntities()`

**Fix Applied**: Marked class as obsolete. Data access moved to component code-behinds.

**Automation Opportunity**: **LOW** - These require manual refactoring to DI patterns.

---

### 9. Layout Mismatch (LayoutComponentBase vs ComponentBase)

**Error**: `CS0263: Partial declarations must not specify different base classes`

**Cause**: Layer 2 generated `MainLayout : ComponentBase` but `.razor` uses `@inherits LayoutComponentBase`.

**Fix Applied**: Changed code-behind to inherit from `LayoutComponentBase`.

**Automation Opportunity**: **HIGH** - Layer 2 should detect layout files (files in `Layout/` folder or with `@inherits LayoutComponentBase`) and not transform them, or use correct base class.

---

### 10. ajaxToolkit:AutoCompleteExtender

**Issue**: Component not in BWFC

**Cause**: AJAX Control Toolkit components have no BWFC equivalent.

**Fix Applied**: Removed markup, added comment noting it was removed.

**Automation Opportunity**: **MEDIUM** - Layer 1 could:
- Remove `<ajaxToolkit:*>` elements
- Insert `@* AjaxToolkit control removed - not available in BWFC *@` comment

---

## Controls Not in BWFC

| Control | Web Forms Namespace | Migration Strategy |
|---------|---------------------|-------------------|
| AutoCompleteExtender | ajaxToolkit | Remove; use JavaScript autocomplete or Blazor component library |
| ScriptManager | asp | Kept as no-op BWFC component |
| UpdatePanel/ContentTemplate | asp | Kept as no-op BWFC components (warnings benign) |
| CommandField | asp | Partial support - ShowDeleteButton/ShowEditButton work |

## Test Results

Application starts successfully:
- Database created and seeded automatically
- All pages render without runtime errors
- Navigation works correctly

**Acceptance Tests**: 31/40 passed (77.5%)

| Category | Passed | Failed |
|----------|--------|--------|
| About Page | 5/5 | 0 |
| Instructors Page | 5/5 | 0 |
| Courses Page | 5/6 | 1 (DetailsView search) |
| Students Page | 7/9 | 2 (Add form, Clear button) |
| Navigation | 2/7 | 5 (nav link assertions) |
| Home Page | 7/8 | 1 (page structure) |

**Key Findings:**
- Adding `@page "/Page.aspx"` fallback routes enables legacy URL support
- Nav link tests fail because Blazor nav uses different element structure than Web Forms
- Form functionality tests fail because Add/Clear buttons need event wiring

## Recommendations (Prioritized)

### Critical (Fix in Layer 2 Script)

1. **Fix `public or private` code generation bug** - This breaks every migration
2. **Handle LayoutComponentBase files** - Don't transform or use correct base class
3. **EF Core DbContext generation** - Convert EF6 patterns properly

### High Priority (Layer 1 Improvements)

4. **Normalize boolean case** - `True`→`true`, `False`→`false`
5. **Handle hex color values** - Quote or convert to named colors
6. **Add ItemType to generic components** - Infer from context where possible
7. **Remove ajaxToolkit elements** - Add removal comments

### Medium Priority

8. **Transform inline style elements** - `<RowStyle>` → `<RowStyleContent><GridViewRowStyle/></RowStyleContent>`
9. **Handle Unit-typed properties** - `BorderWidth`, `Width`, `Height`
10. **Add route fallbacks for .aspx URLs** - Or update test patterns

### Low Priority

11. **Document business logic refactoring** - Generate TODO comments for legacy classes
12. **Warn about unsupported attributes** - `UpdateMethod`, `DeleteMethod`, `SelectMethod`

## Files Modified

| File | Changes |
|------|---------|
| `Models/Model1.Context.cs` | Complete rewrite to EF Core |
| `About.razor` | Removed styles, fixed Items binding |
| `About.razor.cs` | Complete rewrite |
| `Courses.razor` | Removed AutoComplete, fixed bindings |
| `Courses.razor.cs` | Complete rewrite |
| `Students.razor` | Removed AutoComplete, styles, fixed bindings |
| `Students.razor.cs` | Complete rewrite |
| `Instructors.razor` | Fixed Items binding |
| `Instructors.razor.cs` | Complete rewrite |
| `Home.razor.cs` | Simplified (static page) |
| `Components/Layout/MainLayout.razor` | Fixed script tag, links, added @Body |
| `Components/Layout/MainLayout.razor.cs` | Fixed base class |
| `Models/Enrollmet_Logic.cs` | Marked obsolete |
| `Program.cs` | Added DB seed logic |
| `_Imports.razor` | Added Models namespace |

## Time Analysis

| Phase | Time |
|-------|------|
| Layer 1 Script | ~1.5s |
| Layer 2 Script | ~2s |
| Build + Error Analysis | ~5m |
| Manual Fixes | ~45m |
| Testing/Verification | ~10m |
| **Total** | **~1 hour** |

**Estimate with Script Fixes**: If the high-priority issues were automated, manual intervention would drop to ~15 minutes for review and edge cases.
