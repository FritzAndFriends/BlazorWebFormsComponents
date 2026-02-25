# Decision: All 5 Missing Smoke Tests Added (Issue #358)

**Decided by:** Colossus  
**Date:** 2026-02-25

## Context

M9 audit identified 5 sample pages without smoke tests. Issue #358 requested adding them.

## Decision

Added 5 InlineData entries to existing Theory smoke tests in `ControlSampleTests.cs`:

| Page | Theory Method | Route |
|------|--------------|-------|
| ListView CrudOperations | `DataControl_Loads_WithoutErrors` | `/ControlSamples/ListView/CrudOperations` |
| Label | `EditorControl_Loads_WithoutErrors` | `/ControlSamples/Label` |
| Panel BackImageUrl | `EditorControl_Loads_WithoutErrors` | `/ControlSamples/Panel/BackImageUrl` |
| LoginControls Orientation | `LoginControl_Loads_WithoutErrors` | `/ControlSamples/LoginControls/Orientation` |
| DataGrid Styles | `DataControl_Loads_WithoutErrors` | `/ControlSamples/DataGrid/Styles` |

## Notes

- All 5 sample pages confirmed to exist at their routes.
- Panel/BackImageUrl uses external placeholder URLs — smoke test works due to existing "Failed to load resource" filter, but the page should be updated to use local images per team convention.
- No new Fact tests needed; all fit cleanly as InlineData on existing Theory methods.
- Build verified green.
