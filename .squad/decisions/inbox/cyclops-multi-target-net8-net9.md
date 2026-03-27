# Decision: Multi-Target net8.0, net9.0, net10.0

**Author:** Cyclops  
**Date:** 2025-07-26  
**Issue:** #516  

## Decision

BlazorWebFormsComponents, BlazorAjaxToolkitComponents, and the test project now multi-target `net8.0;net9.0;net10.0`. The NuGet package ships assemblies for all three TFMs.

## Rationale

Teams on .NET 8 LTS or .NET 9 need the library without upgrading their runtime. All source code is compatible with C# 12 (net8.0 minimum) — no `#if` guards needed.

## Key Design Choices

1. **Conditional version properties in Directory.Build.props** — `AspNetCoreVersion` resolves to 8.0.0/9.0.0/10.0.0 per TFM. Unconditional 10.0.0 default remains for single-target sample projects.
2. **Sample projects stay net10.0-only** — no need to multi-target apps that exist for demonstration.
3. **SharedSampleObjects unchanged** — its `netstandard2.0` TFM already covers net8.0/net9.0.
4. **CI installs all three SDKs** — `setup-dotnet@v4` with multi-line `dotnet-version`.

## Impact

- Library consumers can now target net8.0 or net9.0
- CI build time increases slightly (3 TFMs instead of 1)
- No breaking changes to existing net10.0 consumers
