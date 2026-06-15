# bishop History


## Learnings

### 2026-06-12T09:56:19-04:00: Audit gate — first repeatable coverage report

- Created `migration-toolkit/scripts/audit-coverage.ps1` — a self-contained PowerShell script that cross-checks `tracked-components.json` against five coverage signals: mkdocs.yml nav, bUnit test folders, ComponentCatalog.cs entries, ControlSampleTests.cs InlineData routes, and README links.
- Wrote the dated output to `dev-docs/audit-report.md` (regenerated on each run).
- First run results (2026-06-12, 61 tracked components): docs 60/61, bUnit folders 41/61 (group-folder caveat noted), catalog 58/61, README 56/61, untested catalog entries 1 (ScriptManagerProxy).
- bUnit "missing" count is conservative: `LoginControls/` and `Validations/` group folders make 13 components appear uncovered even when tests exist — documented as a known methodology note in the report.
- `BaseValidator` and `BaseCompareValidator` are legitimately absent from catalog and README (abstract base classes); `Xml` is deferred — all three flagged by the report as expected gaps.

📋 Team update: Audit gate established — run `migration-toolkit/scripts/audit-coverage.ps1` after each remediation wave to regenerate `dev-docs/audit-report.md` and track progress.


### 2026-06-10T10:33:04.9872011-04:00: #557 completed and #549/#550 scaffolding hooks started

- `WebConfigAssemblyParser.ParseProject()` now merges control registrations across all discovered `Web.config` files (root + nested), not just one file, and emits a shared `PrefixToNamespaceMap`.
- `RuntimeDetector` now surfaces `CustomControlPrefixToNamespaceMap` plus `CodeOnlyServerControls` discovered from `.cs` files inheriting Web Forms server-control bases without markup companions.
- `MigrationContext` and `FileMetadata` now carry `CustomControlPrefixToNamespace` so future markup transforms (issue #550) can consume prefix resolution without re-parsing config.
- `MigrationPipeline` now wires a `CodeOnlyControlScaffolder` skeleton emission step (issue #549 starter) that generates placeholder components under `Generated/CodeOnlyControls` and logs manual follow-up items.

📋 Team update (2026-06-10): Decision merged — "Custom control parser + scaffolder handoff (#557, #549, #550, #548)" with handoff checklist for #549/#550 follow-on work — decided by Bishop
