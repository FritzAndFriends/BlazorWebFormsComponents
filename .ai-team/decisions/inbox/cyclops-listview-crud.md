# Decision: ListView CRUD Events Pattern

**By:** Cyclops
**Date:** 2026-02-25
**Issue:** #356

## What

ListView now has full event parity for CRUD operations. The remaining 7 missing events (Sorting, Sorted, PagePropertiesChanging, PagePropertiesChanged, SelectedIndexChanging, SelectedIndexChanged, LayoutCreated fix) have been implemented following the established patterns.

## Key Decisions

1. **Sort commands route through HandleCommand:** `HandleCommand("Sort", sortExpression, itemIndex)` routes to the Sorting/Sorted event pair. The sort expression is passed as `commandArgument`, consistent with how Web Forms bubbles sort commands.

2. **Select commands route through HandleCommand:** `HandleCommand("Select", null, itemIndex)` routes to SelectedIndexChanging/SelectedIndexChanged events.

3. **PagePropertiesChanging/Changed use SetPageProperties():** Unlike sort/select which route through HandleCommand, paging uses a dedicated `SetPageProperties(startRowIndex, maximumRows)` public method. This matches Web Forms where the DataPager calls this method directly.

4. **LayoutCreated converted from EventHandler to EventCallback:** The original `EventHandler OnLayoutCreated` was never wired and doesn't fit the Blazor EventCallback pattern. Converted to `EventCallback<EventArgs>` for consistency with all other events.

5. **SortDirection toggle matches GridView:** When sorting by the same expression that's already sorted ascending, it toggles to descending. Different expression resets to ascending.

## Why

ListView needs full event parity with Web Forms to support migration scenarios where applications rely on these events for sorting, paging, and selection behavior.
