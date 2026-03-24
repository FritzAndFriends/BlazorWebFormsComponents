# Decision: ViewState/IsPostBack Test Coverage — Breaking Changes Identified

**Author:** Rogue (QA Analyst)  
**Date:** 2026-03-24  
**Status:** FYI — action needed by Cyclops or whoever merges the branch  

## Context

While writing 73 contract tests for the ViewState-PostBack-Shim feature, I identified **3 existing tests that will break** due to intentional behavioral changes:

## Breaking Tests

1. **`WebFormsPageBase/ViewStateTests.razor` → `ViewState_NonExistentKey_ThrowsKeyNotFoundException`**  
   Old behavior: `Dictionary<string, object>` throws on missing key.  
   New behavior: `ViewStateDictionary` returns `null` for missing keys (matches Web Forms semantics).

2. **`WebFormsPageBase/ViewStateTests.razor` → `ViewState_HasObsoleteAttribute`**  
   Old behavior: ViewState has `[Obsolete]` attribute.  
   New behavior: `[Obsolete]` removed — ViewState is now a real feature.

3. **`WebFormsPageBase/WebFormsPageBaseTests.razor` → `IsPostBack_AlwaysReturnsFalse`**  
   Old behavior: `IsPostBack` hardcoded to `false`.  
   New behavior: Mode-adaptive — returns `true` after initialization in InteractiveServer mode.

## Recommendation

These tests should be updated (not deleted) to reflect the new contract. My new tests in `ViewStateDictionaryTests.cs` and `IsPostBackTests.cs` already define the correct behavior.

## Additional Notes

- Added `InternalsVisibleTo` to main csproj for test project access to internal ViewStateDictionary members
- `IDataProtectionProvider` (via `EphemeralDataProtectionProvider`) must be registered in test contexts that render BaseWebFormsComponent-derived components — the `BlazorWebFormsTestContext` base class may need updating
