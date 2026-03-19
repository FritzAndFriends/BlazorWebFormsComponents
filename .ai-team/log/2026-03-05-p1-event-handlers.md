# P1 Event Handler Improvements

**Date:** 2026-03-05
**Requested by:** Jeffrey T. Fritz

## What Happened

Cyclops implemented all 10 P1 event handler improvements from Forge's audit:

| P1 | Issue | Component | Status |
|----|-------|-----------|--------|
| P1-1 | Button OnClick MouseEventArgs → EventArgs | ButtonBaseComponent | ✅ |
| P1-2 | Add Sender property to 32 EventArgs classes | All EventArgs | ✅ |
| P1-3 | GridView PageIndexChanging event | GridView | ✅ |
| P1-4 | GridView RowUpdated/RowDeleted events | GridView | ✅ |
| P1-5 | GridView SelectedIndexChanged int → GridViewSelectEventArgs | GridView | ✅ |
| P1-7 | SelectMethod lambda-friendly (SelectItems parameter) | DataBoundComponent | ✅ |
| P1-8 | SelectMethodAsync async variant | DataBoundComponent | ✅ |
| P1-9 | TreeNodePopulate event | TreeView | ✅ |
| P1-10 | TreeView 4 bare event aliases | TreeView | ✅ |
| P1-11 | DataGrid 5 bare event aliases | DataGrid | ✅ |

Rogue wrote 51 new bUnit tests across 10 test files covering all P1 items.

All 1570 tests pass. Committed as 36a06013, pushed to origin, PR #421 updated.

## Agents

- **Cyclops** — implemented all P1 event handler improvements
- **Rogue** — wrote 51 bUnit tests for P1 items

## Key Outcomes

- P0 + P1 event handler fidelity complete (18 items total)
- Test suite grew from 1519 → 1570
- PR #421 covers LoginView/LoginStatus redesign + P0 + P1
