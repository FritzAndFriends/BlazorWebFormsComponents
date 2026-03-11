# ContosoUniversity Migration — Run 10 Report

## Executive Summary

**Run 10 revealed significant script gaps** when migrating ContosoUniversity — a Web Forms app with different patterns than WingtipToys. The migration scripts work well for WingtipToys-style apps but need enhancements for ContosoUniversity's inline styles and EF6 patterns.

| Metric | Value |
|--------|-------|
| **Build Status** | ❌ FAILED (68 errors) |
| **Layer 1 Time** | 1.17 seconds |
| **Layer 2 Time** | 0.68 seconds |
| **Date** | 2026-03-09 |
| **Branch** | `squad/audit-docs-perf` |

## Timing Details

| Phase | Duration |
|-------|----------|
| Layer 1 (bwfc-migrate.ps1) | 1.17s |
| Layer 2 (bwfc-migrate-layer2.ps1) | 0.68s |
| **Total Script Time** | **1.85s** |
| Build (failed) | 8.0s |

## Script Bug Fixed During Run

**Layer 2 line 1323:** `Get-ChildItem` returned a single object instead of array, causing `.Count` property error. Fixed by wrapping with `@()`:
```powershell
# Before (bug)
$edmxFiles = Get-ChildItem -Path $SourcePath -Filter '*.edmx' ...
# After (fix)
$edmxFiles = @(Get-ChildItem -Path $SourcePath -Filter '*.edmx' ...)
```

## Build Errors Discovered

### Category 1: Style Attribute Quoting (42 errors)
The source app uses inline BWFC style attributes like:
```xml
BackColor="White" ForeColor="#333333" BorderStyle="Solid"
```

In Razor, these parse as C# identifiers (`White`, `Solid`, `Center`) instead of string literals. The script needs to quote these properly:
```razor
BackColor="White"  ❌ Parses White as C# variable
BackColor=@("White")  ✅ Explicit string
```

**Affected attributes:** BackColor, ForeColor, BorderColor, BorderStyle, HorizontalAlign, Font-Bold, etc.

### Category 2: Hex Color Values (15 errors)
Hex colors like `ForeColor="#333333"` cause preprocessor directive errors because `#` starts a C# preprocessor directive in certain contexts.

### Category 3: Code-Behind Entity References (9 errors)
Layer 2's Pattern A generated incorrect entity references:
- `db.Items` — DbContext doesn't have an `Items` property
- `db.Enrollmet_Logics` — Non-existent DbSet name
- `db.strings` — Incorrectly detected entity type

### Category 4: Event Handler Binding (2 errors)
Methods referenced in `OnClick` attributes don't exist in the generated code-behind:
- `btnSearchCourse_Click` 
- `search_Click`

### Category 5: DbContext Constructor (1 error)
The `Enrollmet_Logic.cs` model tries to instantiate `ContosoUniversityEntities()` without the required `DbContextOptions` parameter (EF Core requires DI).

## Root Cause Analysis

### ContosoUniversity vs WingtipToys

| Aspect | WingtipToys | ContosoUniversity |
|--------|-------------|-------------------|
| **Inline Styles** | None (CSS classes) | Heavy use (40+ attributes) |
| **Color Values** | None | Hex colors everywhere |
| **EF Pattern** | Code-First | DB-First (.edmx) |
| **GridView Usage** | Simple | Complex with nested styles |
| **Table Controls** | Minimal | Heavy use of `<Table>` |
| **SelectMethod** | Standard pattern | Non-standard patterns |

The migration scripts were optimized for WingtipToys' patterns. ContosoUniversity uses significantly different patterns that require script enhancements.

## Required Script Enhancements

### Layer 1 Fixes Needed

1. **Quote all BWFC style attribute values**
   - Detect known style attribute names (BackColor, ForeColor, etc.)
   - Ensure values are quoted as strings: `BackColor="White"` → `BackColor=@("White")`
   
2. **Handle hex color values**
   - Convert `ForeColor="#333333"` to `ForeColor=@("#333333")` 
   - Or: Remove inline colors, add TODO to move to CSS

3. **Validate SelectMethod references**
   - Ensure methods exist in code-behind
   - Generate stubs if missing

### Layer 2 Fixes Needed

1. **Improve entity type detection**
   - Don't generate `db.Items` as a fallback
   - Parse actual DbSet properties from DbContext
   
2. **Handle EF6 → EF Core DbContext properly**
   - Remove `DbModelBuilder` references
   - Add parameterless constructor option for legacy patterns
   - Or: Generate proper DI-based instantiation

## Recommendations

1. **ContosoUniversity is not a good BWFC migration candidate** — it uses patterns that don't map cleanly to BWFC (heavy inline styles, DB-first EF, non-standard code-behind).

2. **Enhance scripts for broader compatibility** — but prioritize WingtipToys-style apps since those are the common migration path.

3. **Add script validation** — detect problematic patterns early and warn users before migration starts.

## Files Produced

```
samples/AfterContosoUniversity/
├── About.razor                     # 68 build errors
├── About.razor.cs
├── Courses.razor
├── Courses.razor.cs
├── Home.razor
├── Home.razor.cs
├── Instructors.razor
├── Instructors.razor.cs
├── Students.razor
├── Students.razor.cs
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── MainLayout.razor.cs
│   └── App.razor
├── Models/
│   ├── Model1.Context.cs           # EF Core DbContext (fixed)
│   ├── *.cs                        # Entity models
├── Program.cs                      # Generated by Layer 2
├── ContosoUniversity.csproj        # With project reference to BWFC
├── layer2-manual-items.md
├── layer2-transforms.log
└── scaffold-command.txt
```
