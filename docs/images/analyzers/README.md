# Analyzer Screenshots

This directory should contain Visual Studio screenshots showing the BWFC Roslyn analyzer experience.

## Required Screenshots

| File | Description |
|---|---|
| `bwfc002-info-squiggle.png` | BWFC002 — Green info squiggle on `ViewState["key"]` usage, showing tooltip message |
| `bwfc003-info-squiggle.png` | BWFC003 — Green info squiggle on `IsPostBack` check, showing tooltip message |
| `bwfc025-warning-squiggle.png` | BWFC025 — Yellow warning squiggle on `ViewState["key"] = new DataTable()`, showing tooltip message |

## How to Capture

1. Open a C# file with the relevant pattern in Visual Studio 2022
2. Hover over the squiggle to show the tooltip
3. Use Windows Snipping Tool (Win+Shift+S) to capture the editor area
4. Save as PNG, approximately 800x200 pixels
5. Place in this directory with the filename from the table above
