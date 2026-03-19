# Decision: Navigation UX Improvements for AfterBlazorServerSide Sample App

**Date:** 2026-03-15  
**Agent:** Jubilee (Sample Writer)  
**Requested by:** Jeffrey T. Fritz

## Context

The AfterBlazorServerSide sample app's component navigation had grown to include 20+ AJAX-related controls. The catalog displayed components in insertion order, resulting in an unsorted AJAX section. Additionally, the desktop view expanded ALL categories by default, creating an overly cluttered navigation panel.

## Decision

Implement two targeted UX improvements:

### 1. Alphabetize Components by Name

**Change:** `ComponentCatalog.cs` method `GetByCategory()`

**Before:**
```csharp
public static IEnumerable<ComponentInfo> GetByCategory(string category) =>
    Components.Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
```

**After:**
```csharp
public static IEnumerable<ComponentInfo> GetByCategory(string category) =>
    Components.Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
        .OrderBy(c => c.Name);
```

**Rationale:** Alphabetical ordering improves component discoverability across all categories. Fixes the out-of-order AJAX section and creates consistent organization throughout the catalog.

### 2. Collapse AJAX Category on Desktop

**Change:** `NavMenu.razor` method `CheckIfDesktopAndExpandCategories()`

**Before:**
```csharp
if (isDesktop)
{
    // Expand all categories on desktop
    foreach (var category in ComponentCatalog.Categories)
    {
        expandedCategories.Add(category);
    }
}
```

**After:**
```csharp
if (isDesktop)
{
    // Expand all categories on desktop except AJAX (too many items)
    foreach (var category in ComponentCatalog.Categories)
    {
        if (!category.Equals("AJAX", StringComparison.OrdinalIgnoreCase))
        {
            expandedCategories.Add(category);
        }
    }
}
```

**Rationale:** The AJAX category contains numerous extender and control components. Starting it collapsed on desktop reduces visual clutter while preserving full access (users can expand as needed). Mobile behavior is unchanged, still expanding only the category containing the current page.

## Trade-offs

- **Pro:** Cleaner desktop navigation, improved discoverability
- **Pro:** Mobile experience unchanged
- **Con:** Users must take one extra click to expand AJAX category on first visit
- **Mitigation:** AJAX section remains clearly visible in the nav; expanding takes a single click

## Implementation Notes

- **Files modified:** 2 files, 3 lines of logic added
- **Build status:** ✅ Clean build (0 errors)
- **Testing:** Manual verification that component catalog sorts alphabetically and desktop nav excludes AJAX from auto-expansion
- **Backward compatibility:** No breaking changes; component routing and sample functionality unchanged

## Approval Status

✅ **Implemented** — Changes verified to compile and function as intended.
