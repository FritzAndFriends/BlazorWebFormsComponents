# Decision: TreeView Enhancement (WI-11 + WI-13 + WI-15)

**Author:** Cyclops  
**Date:** 2026-02-24  
**Status:** Implemented  
**Branch:** milestone7/feature-implementation

## Context

TreeView needed three enhancements implemented together since they all touch the same component: node-level styling, selection support, and expand/collapse programmatic control.

## Decisions

### 1. TreeNodeStyle extends Style (not TableItemStyle)

Web Forms `TreeNodeStyle` inherits from `Style`, not `TableItemStyle`. It adds tree-specific properties (`ChildNodesPadding`, `HorizontalPadding`, `ImageUrl`, `NodeSpacing`, `VerticalPadding`) but NOT `HorizontalAlign`/`VerticalAlign`/`Wrap` from `TableItemStyle`. Followed the same inheritance as Web Forms.

### 2. Sub-component pattern mirrors GridView exactly

Created `ITreeViewStyleContainer` + `UiTreeNodeStyle` + 6 sub-component pairs (`.razor` + `.razor.cs`), following the identical pattern used by `IGridViewStyleContainer` + `UiTableItemStyle` + GridView*Style sub-components. This keeps the codebase consistent.

### 3. Style resolution priority

`GetNodeStyle(node)` resolves: **SelectedNodeStyle** (if selected) > **type-specific style** (RootNodeStyle/ParentNodeStyle/LeafNodeStyle) > **NodeStyle** (fallback). This matches Web Forms behavior.

### 4. Selection via @onclick on text anchor

Rather than wrapping the entire row in a clickable element, selection is wired to the existing text `<a>` element via `@onclick="HandleNodeSelect"`. When `NavigateUrl` is empty, `@onclick:preventDefault` suppresses navigation. This preserves the existing HTML structure.

### 5. ExpandDepth applied in OnInitializedAsync

`ExpandDepth` controls initial expansion only. Applied during `TreeNode.OnInitializedAsync()` — if `Depth >= ExpandDepth` and no user override exists, the node starts collapsed. User clicks override via `_UserExpanded`.

### 6. FindNode uses Value with Text fallback

`FindNode(valuePath)` splits on `PathSeparator` and matches each segment against `node.Value ?? node.Text`. This matches Web Forms behavior where Value defaults to Text if not explicitly set.

### 7. NodeIndent replaces hardcoded 20px

The previously hardcoded `width:20px` in indent `<div>` elements now uses `IndentWidth` (from `TreeView.NodeIndent` parameter, default 20). No visual change for existing usage.

## Files Changed

- **New:** `TreeNodeStyle.cs`, `UiTreeNodeStyle.cs`, `Interfaces/ITreeViewStyleContainer.cs`
- **New:** 6 sub-component pairs: `TreeView{NodeStyle,HoverNodeStyle,LeafNodeStyle,ParentNodeStyle,RootNodeStyle,SelectedNodeStyle}.razor{,.cs}`
- **Modified:** `TreeView.razor`, `TreeView.razor.cs`, `TreeNode.razor`, `TreeNode.razor.cs`

## Risks

- BL0005 warning on `Selected` parameter set outside component (same pattern as GridView selection — acceptable).
- HoverNodeStyle CSS is computed but hover interaction requires JS interop or CSS `:hover` pseudo-class — the style data is available but hover event wiring is deferred to a future WI.
