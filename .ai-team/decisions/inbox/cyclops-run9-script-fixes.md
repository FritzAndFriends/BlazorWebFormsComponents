# Decision: Run 9 Script Fixes — bwfc-migrate.ps1

**By:** Cyclops
**Date:** 2026-03-06
**Requested by:** Jeffrey T. Fritz (via Forge post-mortem)

## What

Implemented 9 fixes (RF-03, RF-04, RF-06, RF-07, RF-08, RF-10, RF-11, RF-12, RF-13) in `migration-toolkit/scripts/bwfc-migrate.ps1` to reduce Layer 2 manual work for Run 9+.

## Key Decisions

1. **New-ProjectScaffold now accepts `$SourcePath`** — used to detect Models/, Account/, Login.aspx, Register.aspx in the source and conditionally add EF Core + Identity package references and Program.cs boilerplate.

2. **EF Core package versions use `10.0.0-*` wildcard** — matches the task specification for .NET 10 compatibility.

3. **DbContext transform removes parameterless constructors with `base("connectionName")`** — not just `string connectionName` parameter constructors. WingtipToys uses `public ProductContext() : base("WingtipToys")` pattern.

4. **Redirect handler detection threshold: <100 chars of markup after directive stripping** — pages with Response.Redirect in code-behind and minimal markup are flagged for minimal API conversion. They still go through normal processing (stub or full conversion).

5. **ListView GroupItemCount check runs BEFORE `ConvertFrom-AspPrefix`** — the regex needs the `asp:` prefix to identify ListView specifically.

6. **Identity/Session blocks in Program.cs are TODO-commented out** — they provide scaffolding hints but don't break compilation. The redirect handler comments are appended after file processing.

## Why

Each fix addresses manual work identified in Run 8 post-mortem. Verified with full migration run against WingtipToys sample — all 9 features produce correct output with zero parse errors.
