# Session: 2026-03-03 — ListView CRUD & Migration Toolkit

**Requested by:** Jeffrey T. Fritz
**Branch:** squad/listview-crud-and-toolkit

## What happened

- **PR #414** (WingtipToys features) merged to dev.
- **Cyclops** fixed 2 ListView CRUD event bugs:
  - `ItemCreated` changed from single-fire `EventCallback` to per-item `EventCallback<ListViewItemEventArgs>`, firing before `ItemDataBound` in both grouped and non-grouped paths.
  - `ItemCommand` now fires for ALL commands before routing to specific handlers (Edit, Delete, Update, etc.), matching Web Forms lifecycle.
  - 43 tests pass.
- **Forge** designed migration toolkit package structure (9 documents in `/migration-toolkit/`). Full design: `planning-docs/MIGRATION-TOOLKIT-DESIGN.md`.
- **Beast** writing toolkit content (6 priority documents).

## Decisions

- ListView CRUD event correctness fixes (Cyclops) — see decisions.md
- Migration toolkit package design (Forge) — see decisions.md
