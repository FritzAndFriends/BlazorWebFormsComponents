# Decision: IIS Express Setup Script for BeforeWebForms HTML Audit

**Date:** 2026-02-26
**By:** Cyclops
**Task:** M11-01

## What

Created `scripts/Setup-IISExpress.ps1` — a PowerShell script that automates the BeforeWebForms sample app setup for the HTML audit under IIS Express with dynamic compilation.

## Key Design Decisions

1. **CodeBehind → CodeFile conversion as temporary runtime changes**: The script modifies .aspx/.ascx/.master files in-place but the `-Revert` switch uses `git checkout` to restore them. These changes must NOT be committed — they're only needed for IIS Express dynamic compilation.

2. **NuGet packages restored to `src/packages/`**: Matches the existing repo convention (the .csproj already references this path). The script copies only the required DLLs to `samples/BeforeWebForms/bin/`.

3. **Roslyn compiler tools copied to `bin/roslyn/`**: The `Microsoft.CodeDom.Providers.DotNetCompilerPlatform` package requires Roslyn compilers at runtime for dynamic compilation. The script copies them from the NuGet package's tools directory.

4. **nuget.exe downloaded on demand**: If `nuget.exe` isn't in PATH, the script downloads it to the repo root. This avoids requiring a global install.

5. **Idempotent**: Running the script multiple times is safe — file conversions are regex-based (no double-application), DLL copies use `-Force`, NuGet restore is naturally idempotent.

## Parameters

| Parameter | Default | Description |
|-----------|---------|-------------|
| `-Port` | 55501 | IIS Express port |
| `-NoBrowser` | false | Suppress browser auto-launch |
| `-Revert` | false | Undo temporary changes and exit |

## Files Changed

- **Created:** `scripts/Setup-IISExpress.ps1`
- **Updated:** `scripts/README.md` (added IIS Express section)
