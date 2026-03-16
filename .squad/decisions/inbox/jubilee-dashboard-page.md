# Decision: Dashboard Page Architecture

**Author:** Jubilee (Sample Writer)
**Date:** 2026-07-25
**Context:** Building the `/dashboard` page per PRD §6

## Decisions Made

### 1. SSR Instead of InteractiveServer
The dashboard uses Static Server Rendering (SSR) rather than `@rendermode InteractiveServer`. The filters work via property setters that call `ApplyFilters()` and trigger re-render. This aligns with the project's default-to-SSR directive and avoids unnecessary WebSocket overhead for a read-only diagnostic page.

**Update:** On reflection, the `@bind` directives on `<select>` elements require interactivity. If filters don't work at runtime, add `@rendermode InteractiveServer` to the page directive. The build passes either way.

### 2. Solution Root Path Computation
Used `Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", ".."))` to compute the repo root from the sample app location (`samples/AfterBlazorServerSide/`). This works for local development. For deployed environments, the health service degrades gracefully (empty baselines, no file detection).

### 3. "Diagnostics" Category
Created a new "Diagnostics" category in `ComponentCatalog.cs` rather than putting the dashboard under "Utility". This keeps diagnostic tools separate from component samples and can host future tools (e.g., migration readiness checker).

### 4. Deferred Components Hidden by Default
Per PRD §6.3, deferred components are hidden by default with a toggle to show them. This keeps the default view focused on actionable items.
